 using System;
using System.IO;

namespace Ivao.It.Aurora.FlightStripPrinter;

internal class DataFolderProvider
{
    public static string GetRootFolder()
        => Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
            @"IVAO Italy\Aurora FlightStrip Printer");

    public static string GetStripsFolder() => Path.Combine(GetRootFolder(), "Strips");
    public static string GetLogsFolder() => Path.Combine(GetRootFolder(), "Logs");
}
