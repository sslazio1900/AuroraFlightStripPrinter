 using System;
using System.IO;

namespace Ivao.It.Aurora.FlightStripPrinter;

internal static class DataFolderProvider
{
    private const string SettingsFileName = "settings.yml";

    private static string GetRootFolder()
        => Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
            @"IVAO Italy\Aurora FlightStrip Printer");

    public static string SettingsFile => Path.Combine(GetRootFolder(), SettingsFileName);
    public static string GetStripsFolder() => Path.Combine(GetRootFolder(), "Strips");
    public static string GetLogsFolder() => Path.Combine(GetRootFolder(), "Logs");
}
