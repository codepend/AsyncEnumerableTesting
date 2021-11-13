using AsyncTesting.Shared;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace AsyncTesting.Server.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WeatherForecastController : ControllerBase
    {
        private static readonly string[] Summaries = new[]
        {
        "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
    };

        private readonly ILogger<WeatherForecastController> _logger;

        public WeatherForecastController(ILogger<WeatherForecastController> logger)
        {
            _logger = logger;
        }

        [HttpGet]
        public async IAsyncEnumerable<WeatherForecast> Get()
        {
            //async IAsyncEnumerable<WeatherForecast> streamWeatherForecastsAsync()
            //{
                for (int dasysFromToday = 1; dasysFromToday < 50; dasysFromToday++)
                {

                    var forecast = new WeatherForecast
                    {
                        Date = DateTime.Now.AddDays(dasysFromToday),
                        TemperatureC = Random.Shared.Next(-20, 55),
                        Summary = Summaries[Random.Shared.Next(Summaries.Length)]
                    };

                    yield return forecast;
                    await Task.Delay(100).ConfigureAwait(false);
                };
            //};
            //return streamWeatherForecastsAsync();
        }
    }
}