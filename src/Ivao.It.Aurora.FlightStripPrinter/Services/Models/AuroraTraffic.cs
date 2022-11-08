using Ivao.It.AuroraConnector.Models;

namespace Ivao.It.Aurora.FlightStripPrinter.Services.Models;

public sealed class AuroraTraffic
{
    public string Callsign { get; }
    public TrafficPosition Pos { get; }
    public Flightplan Flightplan { get; }

    public AuroraTraffic(string callsing, TrafficPosition pos, Flightplan flightplan)
    {
        Callsign = callsing;
        Pos = pos;
        this.Flightplan = flightplan;
    }
}
