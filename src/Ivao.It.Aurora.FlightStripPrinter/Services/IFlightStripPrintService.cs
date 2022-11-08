using Ivao.It.Aurora.FlightStripPrinter.Services.Models;
using Ivao.It.AuroraConnector.Models;
using System.Threading.Tasks;

namespace Ivao.It.Aurora.FlightStripPrinter.Services;

public interface IFlightStripPrintService
{
    Task<string> BindAndConvertToPdf(AuroraTraffic tfc);
    bool PrintWholeDocument(string filePath);
}