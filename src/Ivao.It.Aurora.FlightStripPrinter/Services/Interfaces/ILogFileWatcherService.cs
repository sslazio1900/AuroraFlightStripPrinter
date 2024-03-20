using System.IO;

namespace Ivao.It.Aurora.FlightStripPrinter.Services;

public interface ILogFileWatcherService
{
    FileSystemEventHandler? OnLogfileChanged { get; set; }

    void Start(string logFoldlerPath);
}