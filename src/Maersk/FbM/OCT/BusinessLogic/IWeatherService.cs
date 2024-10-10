using Maersk.FbM.OCT.Controller;
using Maersk.FbM.OCT.Model;

namespace Maersk.FbM.OCT.BusinessLogic;

/// <summary>
/// Defines the contract for the weather service in the abstract.
/// </summary>
public interface IWeatherService
{
    ServiceResult<WeatherAlert> GetAlerts(string state);
}