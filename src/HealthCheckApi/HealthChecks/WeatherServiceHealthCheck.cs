using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using HealthCheckApi.Services;
using Microsoft.Extensions.Diagnostics.HealthChecks;

// https://docs.microsoft.com/en-us/aspnet/core/host-and-deploy/health-checks?view=aspnetcore-5.0

namespace HealthCheckApi.HealthChecks
{
    public class WeatherServiceHealthCheck : IHealthCheck
    {
        private IWeatherService _weatherService;
        public WeatherServiceHealthCheck(IWeatherService weatherService)
        {
            _weatherService = weatherService;
        }
        public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = new CancellationToken())
        {
            try
            {
                if (_weatherService.IsHealthy())
                {
                    return HealthCheckResult.Healthy("All good");
                }
                else
                {
                    return HealthCheckResult.Degraded("Service is unhealthy");
                }
            }
            catch (Exception ex)
            {
                return HealthCheckResult.Unhealthy("Critical failure");
            }
            
        }
    }
}
