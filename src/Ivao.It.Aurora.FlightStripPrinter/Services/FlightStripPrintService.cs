using Ivao.It.Aurora.FlightStripPrinter.Models;
using Ivao.It.Aurora.FlightStripPrinter.Services.Models;
using Ivao.It.AuroraConnector.Models;
using Ivao.It.FlightStripper;
using Microsoft.Extensions.Logging;
using Syncfusion.Windows.PdfViewer;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace Ivao.It.Aurora.FlightStripPrinter.Services;

public sealed class FlightStripPrintService : IFlightStripPrintService
{
    private string? _printQueueName;
    private static readonly Regex FixRegex = new Regex("^[A-Z]{3,5}$", RegexOptions.Compiled);
    private static readonly Regex CleanUpRegex = new Regex("\\[[\\w-]*\\]", RegexOptions.Compiled);
    private readonly ISettingsService _settingsService;
    private readonly ILogger<FlightStripPrintService> _logger;
    private SettingsModel? LastSettingsRead;

    public FlightStripPrintService(ISettingsService settingsService, ILogger<FlightStripPrintService> logger)
    {
        _settingsService = settingsService;
        _logger = logger;
    }


    /// <inheritdoc/>
    public async Task<string?> BindAndConvertToPdf(AuroraTraffic tfc, List<AirportConfig> apts)
    {
        HtmlToPdf converter = new();

        try
        {
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

            LastSettingsRead = await _settingsService.GetSettingsAsync();

            html = BindStrip(html, tfc.Flightplan, tfc.Pos, rwy);

            var fileShowed = await converter.CreateStripInPathAsync(tfc.Callsign, html);
            await converter.ConvertToPdfAsync(tfc.Callsign, LastSettingsRead);

            _logger.LogDebug("Strip generated for {callsign}", tfc.Callsign);
            return fileShowed;
        }
        catch(Exception ex)
        {
            _logger.LogError(ex, "Error binding/printing strip for {callsign}", tfc.Callsign);
            throw;
        }
    }

    /// <inheritdoc/>
    public bool PrintWholeDocument(string filePath, bool forcePrinterChoice = false)
    {
        PdfViewerControl viewer = new PdfViewerControl();
        viewer.Load(filePath.Replace(".html", ".pdf"));

        if (!SetPrinterQueue(forcePrinterChoice)) return false;

        //Print
        viewer.PrinterSettings.PageSize = PdfViewerPrintSize.CustomScale;
        viewer.PrinterSettings.ShowPrintStatusDialog = true;
        //viewer.PrinterSettings.ScalePercentage = 190f;
        viewer.PrinterSettings.ScalePercentage = LastSettingsRead?.PrintZoom ?? 0;
        try
        {
            viewer.Print(_printQueueName);
        }
        catch (Exception e)
        {
            //TODO Separate log from user log showed pup
            _logger.LogError(e, "Failed to print strip");
            return false;
        }
        return true;
    }

    private bool SetPrinterQueue(bool forcePrinterChoice = false)
    {
        PrintDialog dialog = new PrintDialog();
        if (
            (forcePrinterChoice || _printQueueName is null)
            &&
            (dialog.ShowDialog() ?? false)
            )
        {
            _printQueueName = dialog.PrintQueue.Name;
            return true;
        }
        return false;
    }


    /// <summary>
    /// Binda i dati sulla flightstrip
    /// </summary>
    /// <param name="html"></param>
    /// <param name="fpl"></param>
    /// <param name="pos"></param>
    /// <returns></returns>
    private string BindStrip(string html, Flightplan fpl, TrafficPosition pos, string? rwy)
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

        var depIcao2 = fpl.DepartureIcao.StartsWith(LastSettingsRead?.AreaIcaoCode ?? string.Empty) 
            ? fpl.DepartureIcao?.Substring(2, 2) 
            : fpl.DepartureIcao;

        var arrIcao2 = fpl.ArrivalIcao.StartsWith(LastSettingsRead?.AreaIcaoCode ?? string.Empty)
            ? fpl.ArrivalIcao?.Substring(2, 2)
            : fpl.ArrivalIcao;

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
            .Replace("[dep2]", depIcao2)
            .Replace("[dest]", fpl.ArrivalIcao)
            .Replace("[dest2]", arrIcao2)
            .Replace("[tas]", fpl.CruisingSpeed)
            .Replace("[alt]", fpl.AlternateIcao)
            .Replace("[rte]", route)
            .Replace("[rmk]", fpl.Remarks)
            //.Replace("[pob]", fpl.p)
            .Replace("[eobt]", fpl.DepartureTime)
            .Replace("[eet]", fpl.Eet)
            .Replace("[eta]", eta.ToString("HHmm"))
            .Replace("[endur]", fpl.Endurance)
            .Replace("[proc-wpt]", pos.WaypointLabel)
            .Replace("[afl]", pos.AltitudeLabel)
            .Replace("[exit-fix]", exit)
            .Replace("[entry-fix]", entry)
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
                template = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @$"Templates\template_{trafficType.Cfg!.Icao}_in.html");
                return File.Exists(template)
                    ? template
                    : Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @$"Templates\template_{Consts.AnyTemplate}_in.html");
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
