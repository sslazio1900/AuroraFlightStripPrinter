using Ivao.It.Aurora.FlightStripPrinter.Services.Models;
using Ivao.It.AuroraConnector.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Ivao.It.Aurora.FlightStripPrinter.Services;

public interface IFlightStripPrintService
{
    /// <summary>
    /// Binds the flight strip resolving the dynamic template and created a PDF copy rotated 90°CW ready to be printed
    /// </summary>
    /// <param name="tfc"></param>
    /// <param name="apts"></param>
    /// <returns></returns>
    Task<string?> BindAndConvertToPdf(AuroraTraffic tfc, List<AirportConfig> apts);

    /// <summary>
    /// Prints the PDF file at the corresponding path
    /// </summary>
    /// <param name="filePath"></param>
    /// <param name="forcePrinterChoice"></param>
    /// <returns></returns>
    bool PrintWholeDocument(string filePath, bool forcePrinterChoice = false);
}