using System.Net;
using Maersk.FbM.OCT.BusinessLogic;
using Microsoft.AspNetCore.Mvc;
using Maersk.FbM.OCT.Model;

namespace Maersk.FbM.OCT.Controller.v1 {
    
    /// <summary>
    /// Provides a rest interface for extracting information from the business logic interface IWeatherService.
    /// </summary>
    [ApiController]
    [Route("api/v1/[controller]")]
    public sealed class WeatherController {
        
        private readonly IWeatherService _service;
        
        public WeatherController(IWeatherService service)
        {
            _service = service;
        }
        
        /// <summary>
        /// Enumerate a list of alerts for a NAM state
        /// </summary>
        /// <param name="state">Two letter abbreviation for the state name</param>
        /// <returns>Results.*(WeatherAlert)</returns>
        [Route("alerts")]
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(WeatherAlert))]
        public IResult GetAlerts([FromQuery] string state)
        {
            try
            {
                ServiceResult<WeatherAlert> result = _service.GetAlerts(state);
                if (result.StatusCode == HttpStatusCode.BadRequest)
                    return Results.BadRequest(result.Model);
                if (result.StatusCode == HttpStatusCode.FailedDependency)
                    return Results.Problem(result.Errors[0].Message);
                return Results.Ok(result.Model); 
            }
            catch (Exception e)
            {
                return Results.Problem(e.Message);
            }
        }
    }
}