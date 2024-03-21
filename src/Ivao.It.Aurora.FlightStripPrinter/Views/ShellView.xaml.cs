using Ivao.It.Aurora.FlightStripPrinter.ViewModels;
using MahApps.Metro.Controls;

namespace Ivao.It.Aurora.FlightStripPrinter.Views;

/// <summary>
/// Interaction logic for ShellView.xaml
/// </summary>
public partial class ShellView : MetroWindow
{
    public ShellView()
    {
        InitializeComponent();
    }

    private async void ShowSettings_Click(object sender, System.Windows.RoutedEventArgs e)
    {
        await ((ShellViewModel)this.DataContext).ShowSettings();
    }
}