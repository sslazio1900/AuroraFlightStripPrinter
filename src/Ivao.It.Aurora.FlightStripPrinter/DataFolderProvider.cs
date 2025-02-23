 using System;
using System.IO;
using System.Reflection;

namespace Ivao.It.Aurora.FlightStripPrinter;

internal static class DataFolderProvider
{
    private const string SettingsFileName = "settings.yml";
    private const string StripsFolderName = "Strips";
    private const string TempaltesFolderName = "Templates";
    private const string LogsFolderName = "Logs";



    private static string GetRootFolder()
        => Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
            @"IVAO Italy\Aurora Flight Strip Printer");
    
    public static string SettingsFile => Path.Combine(GetRootFolder(), SettingsFileName);
    public static string GetStripsFolder() => Path.Combine(GetRootFolder(), StripsFolderName);
    public static string GetTemplatesFolder()
#if DEBUG
        => Path.Combine(Environment.CurrentDirectory, TempaltesFolderName);

#else
        => Path.Combine(GetRootFolder(), TempaltesFolderName);
#endif

    public static string GetLogsFolder() => Path.Combine(GetRootFolder(), LogsFolderName);
}
