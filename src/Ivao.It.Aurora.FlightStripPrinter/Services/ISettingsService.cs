using Ivao.It.Aurora.FlightStripPrinter.Models;
using System.Threading.Tasks;

namespace Ivao.It.Aurora.FlightStripPrinter.Services;

public interface ISettingsService
{
    Task<SettingsModel> GetSettingsAsync();
    Task StoreSettingsAsync(SettingsModel settings);
}