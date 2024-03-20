using System.IO;
using System.Linq;

namespace Ivao.It.Aurora.FlightStripPrinter.Services;

public sealed class LogFileWatcherService : ILogFileWatcherService
{
    private FileSystemWatcher? _watcher;

    public LogFileWatcherService()
    {
    }

    public FileSystemEventHandler? OnLogfileChanged { get; set; }

    public void Start(string logFoldlerPath)
    {
        if (OnLogfileChanged is null) return;
        var fileToWatch = GetMostRecentLogFile(logFoldlerPath);
        _watcher = new FileSystemWatcher(fileToWatch.DirectoryName!, fileToWatch.Name);
        _watcher.Changed += OnLogfileChanged;
        _watcher.NotifyFilter = NotifyFilters.LastAccess | NotifyFilters.LastWrite;
        _watcher.EnableRaisingEvents = true;
    }


    public void Stop()
    {
        _watcher?.Dispose();
    }


    private static FileInfo GetMostRecentLogFile(string logFoldlerPath)
    {
        var directory = new DirectoryInfo(logFoldlerPath);
        var myFile = directory.GetFiles()
                     .Where(f => f.Name.StartsWith("log"))
                     .OrderByDescending(f => f.LastWriteTime)
                     .First();
        return myFile;
    }
}
