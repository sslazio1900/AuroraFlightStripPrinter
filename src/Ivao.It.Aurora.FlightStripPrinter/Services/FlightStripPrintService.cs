using Ivao.It.FlightStripper;
using System.Threading.Tasks;
using System;
using System.IO;
using System.Windows.Controls;
using Spire.Pdf;

namespace Ivao.It.Aurora.FlightStripPrinter.Services;

public class FlightStripPrintService : IFlightStripPrintService
{
    private string? _printQueueName;

    public async Task<string> ConvertToPdf(string callsign)
    {
        HtmlToPdf converter = new();

        var template = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"Templates\template_any_in.html");
        var html = await File.ReadAllTextAsync(template);
        var fileShowed = await converter.CreateStripInPathAsync(callsign, html);
        await converter.ConvertToPdfAsync(callsign);

        return fileShowed;
    }


    public bool PrintWholeDocument(string filePath)
    {
        PdfDocument doc = new PdfDocument();
        doc.LoadFromFile(filePath);
        PrintDialog dialogPrint = new PrintDialog();

        //La print queue name può essere salvata per poter poi stampare "silent" senza passare dalla print dialog
        if (_printQueueName is null && (dialogPrint.ShowDialog() ?? false))
        {
            //Print
            doc.PrintSettings.SelectPageRange(1, 1);
            doc.PrintSettings.PrinterName = dialogPrint.PrintQueue.Name;
            _printQueueName = dialogPrint.PrintQueue.Name;
            doc.Print();
            return true;
        }
        return false;
    }
}
