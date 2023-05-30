using Microsoft.Extensions.Diagnostics.HealthChecks;
using System.Threading;
using System.Threading.Tasks;
using HealthCheckSample.Controllers;
using HealthCheckSample.Models;

namespace WeatherHealthCheck
{
    public class WeatherForecastHealthCheck : IHealthCheck
    {
        private readonly WeatherForecastController _weatherForecastController;

        public WeatherForecastHealthCheck(WeatherForecastController weatherForecastController)
        {
            _weatherForecastController = weatherForecastController;
        }

        public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
        {
            try
            {
                // 在這裡添加您的 WeatherForecastController 健康檢查邏輯
                // 根據檢查結果返回適當的 HealthCheckResult

                var weatherForecasts = _weatherForecastController.Get();
                if (weatherForecasts != null && weatherForecasts.Any())
                {
                    return HealthCheckResult.Healthy("API is healthy.");
                }
                else
                {
                    return HealthCheckResult.Unhealthy("API is unhealthy.");
                }
            }
            catch (Exception ex)
            {
                return HealthCheckResult.Unhealthy("API is unhealthy. Exception: " + ex.Message);
            }
        }
    }
}
