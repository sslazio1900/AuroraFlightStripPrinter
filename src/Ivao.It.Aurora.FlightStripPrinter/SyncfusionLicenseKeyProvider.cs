using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;

namespace Ivao.It.Aurora.FlightStripPrinter;

internal sealed class SyncfusionLicenseKeySource : IConfigurationSource
{
    public IConfigurationProvider Build(IConfigurationBuilder builder) => new SyncfusionLicenseKeyProvider();


}

internal sealed class SyncfusionLicenseKeyProvider : ConfigurationProvider
{
    public override void Load()
    {
        Data = new Dictionary<string, string?>(StringComparer.OrdinalIgnoreCase)
        {
            //TODO implement a static class with the constant string of your license key!
            ["SyncfusionLicenseKey"] = SyncfusionLicense.Key,
        };
    }
}

public static class ConfigurationManagerExtensions
{
    public static IConfigurationBuilder AddSyncfusionLicensing(this IConfigurationBuilder builder)
    {
        builder.Add(new SyncfusionLicenseKeySource());
        return builder;
    }
}