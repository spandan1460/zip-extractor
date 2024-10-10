using System.Net;
using System.Text.Json;
using Maersk.FbM.OCT.Controller;
using NLog;
using Maersk.FbM.OCT.Model;

namespace Maersk.FbM.OCT.BusinessLogic;

/// <summary>
/// An implementation of IWeatherService based on a proxy around api.weather.gov. 
/// </summary>
public class ApiWeatherGovService : IWeatherService
{
    private readonly Logger _logger = NLog.LogManager.Setup().GetCurrentClassLogger();
    private static string WEATHER_URL_PREFIX = "https://api.weather.gov/alerts/active?area=";
    
    /// <summary>
    /// Get the alerts for the provided two character state name.
    /// </summary>
    /// <param name="state">The name of the state in 2 character abbreviation form.</param>
    /// <returns>ServiceResult&lt;WeatherAlert&gt; the encapsulated errors and resultant weather alert(s)</returns>
    public ServiceResult<WeatherAlert> GetAlerts(string state)
    {
        ServiceResult<WeatherAlert> reply = new ServiceResult<WeatherAlert>(HttpStatusCode.FailedDependency,
            new Errors(ErrorModuleEnum.ALERT_PROXY_FALURE, HttpStatusCode.FailedDependency, "Unknown failure"));
        _logger.Info("Weather v1/alert GET called with state=" + state);
        if (null == state)
        {
            _logger.Warn("GetAlerts called without a value for query parameter: state");
            reply = new ServiceResult<WeatherAlert>(HttpStatusCode.BadRequest, new Errors(ErrorModuleEnum.ALERT_PROXY_FALURE, HttpStatusCode.BadRequest, "Missing parameter state"));
        }
        else
        {
            try
            {
                HttpClient request = new HttpClient();
                request.DefaultRequestHeaders.Add("User-Agent",
                    "Mozilla/5.0 (Macintosh; Intel Mac OS X 10_15_7) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/116.0.0.0 Safari/537.36");

                var httpResponse = request.GetAsync(WEATHER_URL_PREFIX + state).Result;
                var body = httpResponse.Content.ReadAsStringAsync().Result;

                _logger.Info("Received reply with statusCode=" + reply.StatusCode);
                if (HttpStatusCode.OK == httpResponse.StatusCode)
                {
                    WeatherAlert alert = JsonSerializer.Deserialize<WeatherAlert>(body);
                    reply = new ServiceResult<WeatherAlert>(HttpStatusCode.OK, alert);
                    _logger.Info("Response from weather service response body size=" + body.Length);
                }
                else
                {
                    _logger.Info("Failed reply from weather alert end-point, response body=" + body +
                                 ", replying with service error");
                }
            }
            catch (Exception e)
            {
                _logger.Error("Weather v1/alert GET state=" + state + ", threw exception=", e);
                reply = new ServiceResult<WeatherAlert>(HttpStatusCode.FailedDependency,
                    new Errors(ErrorModuleEnum.ALERT_PROXY_FALURE, HttpStatusCode.FailedDependency, e.Message));
            }
        }

        return reply;
    }
}