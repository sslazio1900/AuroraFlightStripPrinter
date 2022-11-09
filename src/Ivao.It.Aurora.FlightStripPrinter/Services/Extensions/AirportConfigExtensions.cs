using Ivao.It.AuroraConnector.Models;

namespace Ivao.It.Aurora.FlightStripPrinter.Services.Extensions
{
    internal static class AirportConfigExtensions
    {
        public static string? GetRwy(this AirportConfig cfg, bool isArrival)
        {
            if (isArrival && cfg.ArrRwys.Count == 1)
                return cfg.ArrRwys[0];
            if (!isArrival && cfg.DepRwys.Count == 1)
                return cfg.DepRwys[0];

            return null;
        }
    }
}
