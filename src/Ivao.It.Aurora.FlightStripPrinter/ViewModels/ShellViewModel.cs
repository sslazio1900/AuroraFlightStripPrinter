using Caliburn.Micro;

namespace Ivao.It.Aurora.FlightStripPrinter.ViewModels
{
    public class ShellViewModel : Conductor<object>, IViewModel
    {
        protected override async void OnViewLoaded(object view)
        {
            base.OnViewLoaded(view);
            var vm = IoC.Get<FlightStripPrinterViewModel>();
            await this.ActivateItemAsync(vm);
        }
    }
}
