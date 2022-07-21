using System.Diagnostics;
using System.Management;
using System.Security.Cryptography;

namespace ProcessHunter
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;

        public Worker(ILogger<Worker> logger)
        {
            _logger = logger;
        }

        public static int GetHash(string? path)
        {
            if (path == null) return 0;
            FileStream fileStream = new(path, FileMode.Open, FileAccess.Read);
            byte[] hash = SHA1.Create().ComputeHash(fileStream);
            fileStream.Close();
            return (hash[0] & 0xFF)
                    | ((hash[1] & 0xFF) << 8)
                    | ((hash[2] & 0xFF) << 16)
                    | ((hash[3] & 0xFF) << 24);
        }

        protected struct ProcessProperties
        {
            public HashSet<int> Hash;
            public HashSet<int?> Size;
        }

        protected Dictionary<string, ProcessProperties> _ban = new();

        public override Task StartAsync(CancellationToken cancellationToken)
        {
            _ban.Add("War3", new ProcessProperties() { Hash = new(), Size = new() });
            _ban.Add("Fronzen Throne", new ProcessProperties() { Hash = new(), Size = new() });
            _ban.Add("Warcraft III", new ProcessProperties() { Hash = new(), Size = new() });
            return base.StartAsync(cancellationToken);
        }

        protected void StartWatchEventArrived(object sender, EventArrivedEventArgs eventArgs)
        {
            int processID = Convert.ToInt32(eventArgs.NewEvent.Properties["ProcessID"].Value);
            Process process = Process.GetProcessById(processID);
            string name = process.ProcessName;
            string? path = process.MainModule?.FileName;
            int? size = process.MainModule?.ModuleMemorySize;
            if(_ban.ContainsKey(name))
            {
                process.Kill();
                int hash = GetHash(path);
                if (_ban[name].Hash.Contains(hash) == false) _ban[name].Hash.Add(hash);
                if (_ban[name].Size.Contains(size) == false) _ban[name].Size.Add(size);
                return;
            }
            else
            {
                Dictionary<string, ProcessProperties>.ValueCollection ban = _ban.Values;
                foreach (ProcessProperties p in ban)
                {
                    if (p.Size.Contains(size))
                    {
                        int hash = GetHash(path);
                        if (p.Hash.Contains(hash))
                        {
                            process.Kill();
                            return;
                        }
                    }
                }
            }
        }

        protected Task RunHunter(CancellationToken cancellationToken)
        {
            return Task.Run(() =>
            {
                ManagementEventWatcher startWatch = new(
                        new WqlEventQuery("SELECT * FROM Win32_ProcessStartTrace"));
                startWatch.EventArrived += new EventArrivedEventHandler(StartWatchEventArrived);
                startWatch.Start();
                while (!cancellationToken.IsCancellationRequested)
                {
                    Thread.Sleep(1);
                }
                startWatch.Stop();
            }, cancellationToken);
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            try
            {
                Task task = RunHunter(stoppingToken);
                await task;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "{Message}", ex.Message);
                Environment.Exit(1);
            }
        }
    }
}