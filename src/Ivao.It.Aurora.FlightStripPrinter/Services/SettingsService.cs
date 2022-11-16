using Ivao.It.Aurora.FlightStripPrinter.Models;
using System.IO;
using System.Threading.Tasks;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace Ivao.It.Aurora.FlightStripPrinter.Services;

public class SettingsService : ISettingsService
{
    public async Task<SettingsModel> GetSettingsAsync()
    {
        if (!File.Exists(DataFolderProvider.SettingsFile))
        {
            return new SettingsModel();
        }
        var yaml = await File.ReadAllTextAsync(DataFolderProvider.SettingsFile);
        var ser = new DeserializerBuilder().WithNamingConvention(CamelCaseNamingConvention.Instance).Build();
        return ser.Deserialize<SettingsModel>(yaml);
    }


    public async Task StoreSettingsAsync(SettingsModel settings)
    {
        var ser = new SerializerBuilder().WithNamingConvention(CamelCaseNamingConvention.Instance).Build();
        var yaml = ser.Serialize(settings);
        await File.WriteAllTextAsync(DataFolderProvider.SettingsFile, yaml);
    }

    public async Task InitNewSettingsIfNotExisting()
    {
        if (File.Exists(DataFolderProvider.SettingsFile)) return;

        var settings = new SettingsModel
        {
            AreaIcaoCode = "LI",
            MarginBottom = 16,
            PrintZoom = 100,
            StripHeigth = 65,
            StripWidth = 300,
        };
        await StoreSettingsAsync(settings);
    }
}
