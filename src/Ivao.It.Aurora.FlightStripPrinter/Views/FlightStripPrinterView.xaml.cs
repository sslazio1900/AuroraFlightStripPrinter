using Ivao.It.Aurora.FlightStripPrinter.ViewModels;
using System.Windows;
using System.Windows.Controls;

namespace Ivao.It.Aurora.FlightStripPrinter.Views;

/// <summary>
/// Interaction logic for FlightStripPrinterView.xaml
/// </summary>
public partial class FlightStripPrinterView : UserControl
{
    public FlightStripPrinterView()
    {
            InitializeComponent();

            this.Loaded += (s, e) =>
            {
                PresentationSourceProvider.Current = PresentationSource.FromVisual(this);
                ((FlightStripPrinterViewModel)this.DataContext).UiBrowser = WBrowser;
            };
        }
}