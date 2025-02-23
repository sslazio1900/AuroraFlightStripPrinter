using System;
using Caliburn.Micro;
using Ivao.It.Aurora.FlightStripPrinter.Models;
using Ivao.It.Aurora.FlightStripPrinter.Services;
using System.Threading.Tasks;

namespace Ivao.It.Aurora.FlightStripPrinter.ViewModels;

public class SettingsViewModel : Screen, IViewModel
{
    private readonly ISettingsService _settingsService;

    private SettingsModel? _settings;
    public SettingsModel? Settings
    {
        get => _settings; 
        set
        {
            _settings = value;
            NotifyOfPropertyChange();
        }
    }

    public SettingsViewModel(ISettingsService settingsService)
    {
        _settingsService = settingsService;
    }

    public async Task ViewLoadedAsync()
        => Settings = await _settingsService.GetSettingsAsync();

    public EventHandler<EventArgs>? OnClosed { get; set; }

    public async Task SaveSettingsAndClose()
    {
        if (Settings is null) return;
        await _settingsService.StoreSettingsAsync(Settings);
        await this.TryCloseAsync();
    }
}
