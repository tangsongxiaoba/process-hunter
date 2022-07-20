using Standart.Hash.xxHash;
using System.Diagnostics;
using System.Management;

namespace ProcessHunter
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;

        public Worker(ILogger<Worker> logger)
        {
            _logger = logger;
        }

        public static ulong GetHash(string path) //Standart.Hash.xxHash
        {
            if (path == null) return 0;
            FileStream fileStream = new(path, FileMode.Open, FileAccess.Read);
            var xxh64 = xxHash64.ComputeHash(fileStream);
            fileStream.Close();
            return xxh64;
        }

        protected class ProcessProperties
        {
            public string? Name;
            public List<ulong>? Hash;
            public List<int>? Size;
            public List<string>? Path;
        }

        protected List<ProcessProperties> ban = new();

        public override Task StartAsync(CancellationToken cancellationToken)
        {
            ProcessProperties[] _ban = new ProcessProperties[]
            {
                new ProcessProperties{ Name = "War3", Hash = new(),Size = new(), Path = new()},
                new ProcessProperties{ Name = "Warcraft III", Hash = new(),Size = new(), Path = new()},
                new ProcessProperties{ Name = "Frozen Throne", Hash = new(),Size = new(), Path = new()},
            };
            ban.AddRange(_ban);
            return base.StartAsync(cancellationToken);
        }

        protected void StartWatchEventArrived(object sender, EventArrivedEventArgs eventArgs)
        {
            int processID = Convert.ToInt32(eventArgs.NewEvent.Properties["ProcessID"].Value);
            Process process = Process.GetProcessById(processID);
            string name = process.ProcessName;
            string path = process.MainModule.FileName;
            int size = process.MainModule.ModuleMemorySize;
            int len = ban.Count;
            for (int i = 0; i < len; ++i)
            {
                bool flag = false;
                if (name == ban[i].Name) flag = true;
                else
                {
                    if (ban[i].Size.Any(x => x == size))
                    {
                        ulong hash = GetHash(path);
                        if (ban[i].Hash.Any(x => x == hash))
                        {
                            flag = true;
                        }
                    }
                }
                if (flag)
                {
                    ulong hash = GetHash(path);
                    if (ban[i].Hash.Any(x => x == hash) == false) ban[i].Hash.Add(hash);
                    if (ban[i].Size.Any(x => x == size) == false) ban[i].Size.Add(size);
                    if (ban[i].Path.Any(x => x == path) == false) ban[i].Path.Add(path);
                    process.Kill();
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
                    Thread.Sleep(50);
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