using System;

namespace Ivao.It.Aurora.FlightStripPrinter;

/// <summary>
/// Aspnet core like environment
/// </summary>
internal static class EnvironmentHandler
{
    public static string? GetCurrentEnvironment() => Environment.GetEnvironmentVariable("Environment");
    public static bool IsProduction() => "production".Equals(GetCurrentEnvironment(), StringComparison.OrdinalIgnoreCase);
    public static bool IsBeta() => "beta".Equals(GetCurrentEnvironment(), StringComparison.OrdinalIgnoreCase);
    public static bool IsDevelopment() => "debug".Equals(GetCurrentEnvironment(), StringComparison.OrdinalIgnoreCase);
}
