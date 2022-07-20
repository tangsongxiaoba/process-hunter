# 创作背景及原理实现

> qgnb dddd

为了禁止我的一个同学在教室电脑上玩魔兽三，出此下策

所以程序默认查找的进程也是
- "War3.exe"
- "Frozen Throne.exe"
- "Warcraft III.exe"

菜鸟第一次写 C# Orz

辗转反侧找到了 [.Net Process Monitor](https://stackoverflow.com/questions/1986249/net-process-monitor)

使用 [MangementEventWatcher 类](https://docs.microsoft.com/zh-cn/dotnet/api/system.management.managementeventwatcher?view=dotnet-plat-ext-6.0) 和 [Win32_ProcessTrace 类](https://docs.microsoft.com/en-us/previous-versions/windows/desktop/krnlprov/win32-processtrace)

所以基本上就是

1. 查询新进程
2. 判断文件名或文件哈希是否符合
3. 杀死符合的进程
4. 重复步骤

很简单的想法 实现也不困难 有问题搜博客就好了（

性能方面实在拉跨 而且有些进程结束太快就查不到 还有一些拒绝访问 基本上以后要改进的