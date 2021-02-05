using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using HealthCheckApi.HealthChecks;
using HealthCheckApi.Services;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace HealthCheckApi
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {

            services.AddControllers()
                .AddJsonOptions(opts =>
                {
                    // Allow ENUM serialization as strings instead of integers
                    // https://docs.microsoft.com/en-us/dotnet/standard/serialization/system-text-json-customize-properties#enums-as-strings
                    opts.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter(JsonNamingPolicy.CamelCase));
                });
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "HealthCheckApi", Version = "v1" });
            });
            services.AddScoped<IWeatherService, WeatherService>();
            services.AddHealthChecks()
                .AddCheck<WeatherServiceHealthCheck>("Weather Service")
                .AddCheck<TestHealthCheck>("Test Check");
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "HealthCheckApi v1"));
            }

            app.UseHttpsRedirection();
            app.UseRouting();
            app.UseAuthorization();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                // Enable healthchecks
                app.UseEndpoints(endpoints =>
                {
                    endpoints.MapHealthChecks("/health", new HealthCheckOptions()
                    {
                        ResponseWriter = WriteResponse
                    });
                });
            });
        }

        private static Task WriteResponse(HttpContext context, HealthReport r)
        {
            context.Response.ContentType = "application/json; charset=utf-8";
            var resp = JsonSerializer.Serialize(new
            {
                status = r.Status.ToString(), 
                errors = r.Entries.Select(e => new
                {
                    key = e.Key,
                    value = e.Value.Status.ToString(),
                    description = e.Value.Description
                })
            }, new JsonSerializerOptions());
            return context.Response.WriteAsync(resp);
        }
    }
}
