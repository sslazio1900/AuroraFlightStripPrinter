using System.Threading.Tasks;
using System;
using System.IO;
using System.Windows.Controls;
using Spire.Pdf;
using Ivao.It.AuroraConnector.Models;
using System.Text.RegularExpressions;
using System.Linq;
using Ivao.It.Aurora.FlightStripPrinter.Services.Models;

namespace Ivao.It.Aurora.FlightStripPrinter.Services;

public sealed class FlightStripPrintService : IFlightStripPrintService
{
    private string? _printQueueName;
    private static Regex FixRegex = new Regex("^[A-Z]{3,5}$", RegexOptions.Compiled);
    private static Regex CleanUpRegex = new Regex("\\[[\\w-]*\\]", RegexOptions.Compiled);

   

    public async Task<string> BindAndConvertToPdf(AuroraTraffic tfc)
    {
        HtmlToPdf converter = new();

        //TODO Dynamic Templates
        var template = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"Templates\template_any_in.html");
        var html = await File.ReadAllTextAsync(template);
        
        html = BindStrip(html, tfc.Flightplan, tfc.Pos);

        var fileShowed = await converter.CreateStripInPathAsync(tfc.Callsign, html);
        await converter.ConvertToPdfAsync(tfc.Callsign);

        return fileShowed;
    }


    public bool PrintWholeDocument(string filePath)
    {
        PdfDocument doc = new PdfDocument();
        doc.LoadFromFile(filePath.Replace(".html", ".pdf"));
        PrintDialog dialogPrint = new PrintDialog();

        //La print queue name può essere salvata per poter poi stampare "silent" senza passare dalla print dialog
        if (_printQueueName is null && (dialogPrint.ShowDialog() ?? false))
        {
            _printQueueName = dialogPrint.PrintQueue.Name;
        }
        if (_printQueueName is null) return false;

        //Print
        doc.PrintSettings.SelectPageRange(1, 1);
        doc.PrintSettings.PrinterName = dialogPrint.PrintQueue.Name;
        try
        {
            doc.Print();
        }
        catch (Exception)
        {
            return false;
        }
        return true;
    }

    private static string BindStrip(string html, Flightplan fpl, TrafficPosition pos)
    {
        //ETA
        var deptTimeHh = int.Parse(fpl.DepartureTime[..2]);
        var deptTimeMm = int.Parse(fpl.DepartureTime[2..]);
        DateTime depTime = new DateTime(2022, 1, 1, deptTimeHh, deptTimeMm, 0);
        var etaHh = int.Parse(fpl.Eet[..2]);
        var etaMm = int.Parse(fpl.Eet[2..]);
        DateTime eta = depTime.AddHours(etaHh).AddMinutes(etaMm);

        //Entry/Exit
        var routeSegments = fpl.Route.Split(" ", StringSplitOptions.RemoveEmptyEntries);
        var entry = routeSegments.First(i => FixRegex.IsMatch(i));
        var exit = routeSegments.Reverse().First(i => FixRegex.IsMatch(i));

        //Route truncate: primi 3 blocchi (SID WPT AWY) ... ultimi 3 blocchi (AWY WPT STAR)
        var routeChunks = fpl.Route.Split(' ');
        var route = $"{string.Join(' ', routeChunks[..3])} ... {string.Join(' ', routeChunks[^3..])}";

        var strip = html
            .Replace("[cs]", pos.Callsign)
            //.Replace("[vid]", fpl.Vid)
            .Replace("[assr]", pos.SsrLabel)
            .Replace("[ssr]", pos.SsrSet)
            .Replace("[acft-typ]", fpl.AircraftIcao)
            .Replace("[acft-cat]", fpl.AircraftWtc)
            .Replace("[equip]", fpl.Equipment)
            //.Replace("[transp]", fpl.)
            .Replace("[rules]", fpl.FlightRules)
            .Replace("[rfl]", fpl.CruisingAlt)
            .Replace("[rf]", fpl.CruisingAlt.Replace("F", ""))
            .Replace("[dep]", fpl.DepartureIcao)
            .Replace("[dest]", fpl.ArrivalIcao)
            .Replace("[tas]", fpl.CruisingSpeed)
            .Replace("[alt]", fpl.AlternateIcao)
            .Replace("[rte]", route)
            .Replace("[rmk]", fpl.Remarks)
            //.Replace("[pob]", fpl.p)
            .Replace("[eobt]", fpl.DepartureTime)
            .Replace("[eet]", fpl.Eet)
            .Replace("[eta]", eta.ToString("HHmm"))
            .Replace("[endur]", fpl.Endurance)
            //.Replace("[rwy]", traffic.Clearance.Rwy)
            .Replace("[sid]", pos.WaypointLabel)
            .Replace("[afl]", pos.AltitudeLabel)
            .Replace("[exit-fix]", entry)
            .Replace("[entry-fix]", exit)
            .Replace("[stand]", pos.CurrentGate);
        //.Replace("[p-time]", traffic.Clearance.LastPrintTime?.ToString("HHmm"));

        strip = CleanUpRegex.Replace(strip, string.Empty);

        return strip;
    }
}
