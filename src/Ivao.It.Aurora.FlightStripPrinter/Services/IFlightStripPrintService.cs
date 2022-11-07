using System.Threading.Tasks;

namespace Ivao.It.Aurora.FlightStripPrinter.Services;

public interface IFlightStripPrintService
{
    Task<string> ConvertToPdf(string callsign);
    bool PrintWholeDocument(string filePath);
}