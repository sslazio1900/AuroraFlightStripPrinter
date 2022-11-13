using System.Threading.Tasks;

namespace Ivao.It.Aurora.FlightStripPrinter.ViewModels;

internal interface IViewModel
{
    Task ViewLoadedAsync();
}