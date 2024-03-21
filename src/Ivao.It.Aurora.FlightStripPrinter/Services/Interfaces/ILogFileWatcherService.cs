using System.IO;

namespace Ivao.It.Aurora.FlightStripPrinter.Services;

public interface ILogFileWatcherService
{
    FileInfo WatchingFile { get; }
    FileSystemEventHandler? OnLogfileChanged { get; set; }
    void Init(string logFoldlerPath);
    void Start();
}