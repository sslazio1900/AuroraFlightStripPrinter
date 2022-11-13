using Caliburn.Micro;
using Ivao.It.Aurora.FlightStripPrinter.Models;
using System.IO;
using System.Threading.Tasks;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace Ivao.It.Aurora.FlightStripPrinter.ViewModels;

public class SettingsViewModel : Screen, IViewModel
{
    private SettingsModel _settings;
    public SettingsModel Settings
    {
        get => _settings; set
        {
            _settings = value;
            NotifyOfPropertyChange();
        }
    }

    public SettingsViewModel()
    {
    }

    public async Task ViewLoadedAsync()
    {
        if (!File.Exists(DataFolderProvider.SettingsFile))
        {
            Settings= new SettingsModel();
            return;
        }
        var yaml = await File.ReadAllTextAsync(DataFolderProvider.SettingsFile);
        var ser = new DeserializerBuilder().WithNamingConvention(CamelCaseNamingConvention.Instance).Build();
        Settings = ser.Deserialize<SettingsModel>(yaml);
    }

    public async Task SaveSettingsAndClose()
    {
        var ser = new SerializerBuilder().WithNamingConvention(CamelCaseNamingConvention.Instance).Build();
        var yaml = ser.Serialize(Settings);
        await File.WriteAllTextAsync(DataFolderProvider.SettingsFile, yaml);
        await this.TryCloseAsync();
    }
}
