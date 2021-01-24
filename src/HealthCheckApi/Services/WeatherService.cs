using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace HealthCheckApi.Services
{
    public enum WeatherSummary
    {
        Freezing,
        Bracing,
        Chilly,
        Cool,
        Mild,
        Warm,
        Balmy,
        Hot,
        Sweltering,
        Scorching
    }

    public interface IWeatherService
    {
        IEnumerable<WeatherForecast> GetForecast();
        bool IsHealthy();
    }


    public class WeatherService : IWeatherService
    {
        public IEnumerable<WeatherForecast> GetForecast()
        {
            var values = Enum.GetValues(typeof(WeatherSummary));
            var rng = new Random();
            return Enumerable.Range(1, 5).Select(index => new WeatherForecast
                {
                    Date = DateTime.Now.AddDays(index),
                    TemperatureC = rng.Next(-20, 55),
                    Summary = (WeatherSummary)values.GetValue(rng.Next(values.Length))
                })
                .ToArray();
        }

        public bool IsHealthy()
        {
            //throw new Exception("Something went wrong");
            return false;
        }   
    }
}
