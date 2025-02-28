using Ivao.It.Aurora.FlightStripPrinter.ViewModels;
using MahApps.Metro.Controls;
using System;

namespace Ivao.It.Aurora.FlightStripPrinter.Views;

/// <summary>
/// Interaction logic for PrintPreview.xaml
/// </summary>
public partial class PrintPreviewView : MetroWindow
{
    public PrintPreviewView()
    {
        InitializeComponent();
    }

    protected override void OnClosed(EventArgs e)
    {
        base.OnClosed(e);
        ((IViewModel)this.DataContext).OnClosed?.Invoke(this, e);
    }
}