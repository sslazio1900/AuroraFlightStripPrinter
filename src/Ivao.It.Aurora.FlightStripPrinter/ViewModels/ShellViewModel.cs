using Caliburn.Micro;
using Ivao.It.Aurora.FlightStripPrinter.Extensions;
using System.Reflection;
using System.Threading.Tasks;

namespace Ivao.It.Aurora.FlightStripPrinter.ViewModels;

public class ShellViewModel : Conductor<object>, IViewModel
{
    private readonly IWindowManager _winManager;

    public ShellViewModel(IWindowManager winManager)
    {
        _winManager = winManager;
    }

    public string Version => $"v{Assembly.GetExecutingAssembly().GetName().Version!.Major}." +
        $"{Assembly.GetExecutingAssembly().GetName().Version!.Minor}." +
        $"{Assembly.GetExecutingAssembly().GetName().Version!.Build} " +
        $"{(EnvironmentHandler.IsProduction() ? string.Empty : EnvironmentHandler.GetCurrentEnvironment()!.ToUpper())}";

    protected override async void OnViewLoaded(object view)
    {
        base.OnViewLoaded(view);
        var vm = IoC.Get<FlightStripPrinterViewModel>();
        await this.ActivateItemAsync(vm);
    }

    public async Task ShowSettings() => await this.ShowDialogAsync<SettingsViewModel>();

    public Task ViewLoadedAsync() => Task.CompletedTask;
}
