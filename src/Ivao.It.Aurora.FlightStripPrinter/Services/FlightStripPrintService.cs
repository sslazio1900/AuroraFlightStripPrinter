using System.Threading.Tasks;
using System;
using System.IO;
using System.Windows.Controls;
//using Spire.Pdf;
using Ivao.It.AuroraConnector.Models;
using System.Text.RegularExpressions;
using System.Linq;
using Ivao.It.Aurora.FlightStripPrinter.Services.Models;
using System.Collections.Generic;
using Ivao.It.FlightStripper;
using Syncfusion.Windows.PdfViewer;
using Caliburn.Micro;
using System.Printing;

namespace Ivao.It.Aurora.FlightStripPrinter.Services;

public sealed class FlightStripPrintService : IFlightStripPrintService
{
    private string? _printQueueName;
    private static Regex FixRegex = new Regex("^[A-Z]{3,5}$", RegexOptions.Compiled);
    private static Regex CleanUpRegex = new Regex("\\[[\\w-]*\\]", RegexOptions.Compiled);


    /// <inheritdoc/>
    public async Task<string> BindAndConvertToPdf(AuroraTraffic tfc, List<AirportConfig> apts)
    {
        HtmlToPdf converter = new();

        var trafficType = GetTrafficType(tfc, apts);
        var template = GetTemplatePath(trafficType);
        var html = await File.ReadAllTextAsync(template);
        var rwy = trafficType.Type switch
        {
            TrafficType.Departure => trafficType.Cfg?.DepRwys.Count == 1 ? trafficType.Cfg?.DepRwys[0] : null,
            TrafficType.Arrival => trafficType.Cfg?.ArrRwys.Count == 1 ? trafficType.Cfg?.ArrRwys[0] : null,
            TrafficType.Transit => null,
            TrafficType.Vfr => null,
            _ => throw new ArgumentOutOfRangeException()
        };

        html = BindStrip(html, tfc.Flightplan, tfc.Pos, rwy);

        var fileShowed = await converter.CreateStripInPathAsync(tfc.Callsign, html);
        await converter.ConvertToPdfAsync(tfc.Callsign);

        return fileShowed;
    }

    /// <inheritdoc/>
    public bool PrintWholeDocument(string filePath)
    {
        PdfViewerControl viewer = new PdfViewerControl();
        viewer.Load(filePath.Replace(".html", ".pdf"));
        PrintDialog dialog = new PrintDialog();

        //La print queue name può essere salvata per poter poi stampare "silent" senza passare dalla print dialog
        if (_printQueueName is null && (dialog.ShowDialog() ?? false))
        {
            _printQueueName = dialog.PrintQueue.Name;
        }
        if (_printQueueName is null) return false;

        //Print
        viewer.PrinterSettings.PageSize = PdfViewerPrintSize.CustomScale;
        viewer.PrinterSettings.ShowPrintStatusDialog = true;
        viewer.PrinterSettings.ScalePercentage = 190f;
        try
        {
            viewer.Print(_printQueueName);
        }
        catch (Exception)
        {
            return false;
        }
        return true;
    }


    ///// <inheritdoc/>
    //public bool PrintWholeDocument(string filePath)
    //{
    //    PdfDocument doc = new PdfDocument();
    //    doc.LoadFromFile(filePath.Replace(".html", ".pdf"));
    //    PrintDialog dialogPrint = new PrintDialog();

    //    //La print queue name può essere salvata per poter poi stampare "silent" senza passare dalla print dialog
    //    if (_printQueueName is null && (dialogPrint.ShowDialog() ?? false))
    //    {
    //        _printQueueName = dialogPrint.PrintQueue.Name;
    //    }
    //    if (_printQueueName is null) return false;

    //    //Print
    //    doc.PrintSettings.SelectPageRange(1, 1);
    //    doc.PrintSettings.PrinterName = dialogPrint.PrintQueue.Name;
    //    doc.PrintSettings.
    //    try
    //    {
    //        doc.Print();
    //    }
    //    catch (Exception)
    //    {
    //        return false;
    //    }
    //    return true;
    //}




    /// <summary>
    /// Binda i dati sulla flightstrip
    /// </summary>
    /// <param name="html"></param>
    /// <param name="fpl"></param>
    /// <param name="pos"></param>
    /// <returns></returns>
    private static string BindStrip(string html, Flightplan fpl, TrafficPosition pos, string? rwy)
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
        var entry = routeSegments.FirstOrDefault(i => FixRegex.IsMatch(i));
        var exit = routeSegments.Reverse().FirstOrDefault(i => FixRegex.IsMatch(i));

        //Route truncate: primi 3 blocchi (SID WPT AWY) ... ultimi 3 blocchi (AWY WPT STAR)
        var routeChunks = fpl.Route.Split(' ');
        string route = fpl.Route;
        if (routeChunks.Length >= 3)
        {
            route = $"{string.Join(' ', routeChunks[..3])}...{string.Join(' ', routeChunks[^3..])}";
        }

        //TODO CHECK NEXT FROM TRAFFIC POS -> Freq? Nome? Se Freq molto utile...


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
            .Replace("[rf]", fpl.CruisingAlt?.Replace("F", ""))
            .Replace("[dep]", fpl.DepartureIcao)
            .Replace("[dep2]", fpl.DepartureIcao?.Substring(2, 2))
            .Replace("[dest]", fpl.ArrivalIcao)
            .Replace("[dest2]", fpl.ArrivalIcao?.Substring(2, 2))
            .Replace("[tas]", fpl.CruisingSpeed)
            .Replace("[alt]", fpl.AlternateIcao)
            .Replace("[rte]", route)
            .Replace("[rmk]", fpl.Remarks)
            //.Replace("[pob]", fpl.p)
            .Replace("[eobt]", fpl.DepartureTime)
            .Replace("[eet]", fpl.Eet)
            .Replace("[eta]", eta.ToString("HHmm"))
            .Replace("[endur]", fpl.Endurance)
            .Replace("[sid]", pos.WaypointLabel)
            .Replace("[afl]", pos.AltitudeLabel)
            .Replace("[exit-fix]", entry)
            .Replace("[entry-fix]", exit)
            .Replace("[stand]", pos.CurrentGate)
            .Replace("[no-fpl]", fpl.Route.Contains("NO FPL") ? null : "&check;")
            .Replace("[p-time]", DateTime.UtcNow.ToString("HHmm"));

        if (rwy is not null)
        {
            strip.Replace("[rwy]", rwy);
        }

        strip = CleanUpRegex.Replace(strip, string.Empty);

        return strip;
    }

    /// <summary>
    /// Trova il template. Se il volo è domestico, stampa la strip transit.
    /// </summary>
    /// <param name="tfc"></param>
    /// <param name="cfg"></param>
    /// <returns></returns>
    private static string GetTemplatePath((TrafficType Type, AirportConfig? Cfg) trafficType)
    {
        string template;

        switch (trafficType.Type)
        {
            case TrafficType.Departure:
                template = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @$"Templates\template_{trafficType.Cfg!.Icao}_out.html");
                return File.Exists(template)
                    ? template
                    : Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @$"Templates\template_{Consts.AnyTemplate}_out.html");
            case TrafficType.Arrival:
                template = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @$"Templates\template_{trafficType.Cfg!.Icao}_out.html");
                return File.Exists(template)
                    ? template
                    : Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @$"Templates\template_{Consts.AnyTemplate}_out.html");
            case TrafficType.Vfr:
                    return Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @$"Templates\template_{Consts.AnyTemplate}_vfr.html");
            default:
                return Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"Templates\template_trans.html");
        }
    }

    /// <summary>
    /// Partenza, arrivo o transit? Determina il tipo e restituisce l'airport config da usare
    /// </summary>
    /// <param name="tfc"></param>
    /// <param name="apts"></param>
    /// <returns></returns>
    private static (TrafficType Type, AirportConfig? Cfg) GetTrafficType(AuroraTraffic tfc, List<AirportConfig> apts)
    {
        var depApt = apts.FirstOrDefault(a => a.Icao == tfc.Flightplan.DepartureIcao);
        var arrApt = apts.FirstOrDefault(a => a.Icao == tfc.Flightplan.ArrivalIcao);

        if (tfc.Flightplan.FlightRules == "V") return new(TrafficType.Vfr, null);
        if (depApt is not null && arrApt is null) return new(TrafficType.Departure, depApt);
        if (arrApt is not null && depApt is null) return new(TrafficType.Arrival, arrApt);
        return new(TrafficType.Transit, null);
    }

    private enum TrafficType
    {
        Transit,
        Departure,
        Arrival,
        Vfr
    }
}
