using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Ivao.It.Aurora.FlightStripPrinter;

internal class GitHubUpdateManager
{
    private const string LatestReleaseUrl = @"https://github.com/sslazio1900/AuroraFlightStripPrinter/releases/latest";

    public async Task CheckForUpdates()
    {
        var client = new HttpClient();
        client.BaseAddress = new Uri(LatestReleaseUrl);

        var s = await client.GetAsync("");
        var result = await s.Content.ReadAsStringAsync();
    }
}