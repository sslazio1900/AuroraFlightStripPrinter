using Ivao.It.Aurora.FlightStripPrinter.ViewModels;
using MahApps.Metro.Controls;

namespace Ivao.It.Aurora.FlightStripPrinter.Views
{
    public class BaseWindow : MetroWindow
    {
        public BaseWindow()
        {
            Loaded += BaseWindow_Loaded;
        }


        private async void BaseWindow_Loaded(object sender, System.Windows.RoutedEventArgs e) 
            => await ((IViewModel)this.DataContext).ViewLoadedAsync();
    }
}
