using Caliburn.Micro;
using System.Reflection;

namespace Ivao.It.Aurora.FlightStripPrinter.ViewModels
{
    public class ShellViewModel : Conductor<object>, IViewModel
    {
        public string Version => $"v{Assembly.GetExecutingAssembly().GetName().Version!.Major}.{Assembly.GetExecutingAssembly().GetName().Version!.Minor}.{Assembly.GetExecutingAssembly().GetName().Version!.Build}";

        protected override async void OnViewLoaded(object view)
        {
            base.OnViewLoaded(view);
            var vm = IoC.Get<FlightStripPrinterViewModel>();
            await this.ActivateItemAsync(vm);
        }
    }
}
