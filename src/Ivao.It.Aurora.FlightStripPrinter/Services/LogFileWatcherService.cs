using System;
using System.IO;
using System.Linq;

namespace Ivao.It.Aurora.FlightStripPrinter.Services;

public sealed class LogFileWatcherService : ILogFileWatcherService
{
    private FileSystemWatcher? _watcher;
    public FileInfo WatchingFile { get; private set; }


    public FileSystemEventHandler? OnLogfileChanged { get; set; }
    public void Init(string logFoldlerPath)
    {
        WatchingFile = GetMostRecentLogFile(logFoldlerPath);
    }

    public void Start()
    {
        if (OnLogfileChanged is null) return;
        ArgumentNullException.ThrowIfNull(WatchingFile);

        _watcher = new FileSystemWatcher(WatchingFile.DirectoryName!, WatchingFile.Name);
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
