using Microsoft.Extensions.DependencyInjection;

namespace Ivao.It.Aurora.FlightStripPrinter;

public static class ConfigurationManagerExtensions
{
    public static IServiceCollection AddSyncfusionLicensing(this IServiceCollection services)
    {
        Syncfusion.Licensing.SyncfusionLicenseProvider.RegisterLicense(SyncfusionLicense.Key);
        return services;
    }
}