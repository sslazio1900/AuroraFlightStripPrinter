using System;
using Caliburn.Micro;
using System.Threading.Tasks;

namespace Ivao.It.Aurora.FlightStripPrinter.ViewModels;

public class PrintPreviewViewModel() : PropertyChangedBase, IViewModel
{
    private string? _pdfFilePath;

    public string? PdfFilePath
    {
        get => _pdfFilePath;
        set
        {
            if (value == _pdfFilePath)
            {
                return;
            }

            _pdfFilePath = value;
            NotifyOfPropertyChange();
        }
    }

    public EventHandler<EventArgs>? OnClosed { get; set; }
    
    public Task ViewLoadedAsync() => Task.CompletedTask;
    //=> Settings = await settingsService.GetSettingsAsync();
}