using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using Ivao.It.AuroraConnector;
using Ivao.It.AuroraConnector.AuroraMessages.Responses;
using Ivao.It.AuroraConnector.AuroraMessages.Requests;

namespace Ivao.It.Aurora.FlightStripPrinter.Services;

public class AuroraService : IAuroraService
{
    private readonly ILogger<AuroraService> _logger;
    private readonly AuroraThirdPartyConnector _aurora;

    public AuroraService(ILoggerFactory logger)
    {
        _logger = logger.CreateLogger<AuroraService>();
        _aurora = new(logger.CreateLogger<AuroraThirdPartyConnector>());
    }

    public async Task ConnectAsync()
    {
        await _aurora.ConnectAsync();
        _logger.LogInformation("Connected to Aurora");
    }

    public async Task<string?> GetSelectedTrafficAsync()
    {
        var response = await _aurora.SendAsync<SelectedTrafficResponse>(new SelectedTrafficRequest());
        if (response.Callsign is null)
        {
            _logger.LogWarning("No Aurora traffic selected");
        }
        else
        {
            _logger.LogInformation("Traffic selected: {callsign}", response.Callsign);
        }

        return response.Callsign;
    }

    public void Dispose()
    {
        _aurora.Dispose();
    }
}
