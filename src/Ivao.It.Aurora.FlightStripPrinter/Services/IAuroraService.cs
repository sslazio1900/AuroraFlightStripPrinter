using Ivao.It.Aurora.FlightStripPrinter.Services.Models;
using Ivao.It.AuroraConnector.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Ivao.It.Aurora.FlightStripPrinter.Services;

public interface IAuroraService : IDisposable
{
    Task ConnectAsync();
    Task<AuroraTraffic?> GetSelectedTrafficAsync();
    Task<List<AirportConfig>?> GetControlledAirportsAsync();
}