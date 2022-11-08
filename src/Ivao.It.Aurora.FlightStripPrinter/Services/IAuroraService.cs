using Ivao.It.Aurora.FlightStripPrinter.Services.Models;
using System;
using System.Threading.Tasks;

namespace Ivao.It.Aurora.FlightStripPrinter.Services;

public interface IAuroraService : IDisposable
{
    Task ConnectAsync();
    Task<AuroraTraffic?> GetSelectedTrafficAsync();
}