using Microsoft.Extensions.Diagnostics.HealthChecks;
using System.Threading;
using System.Threading.Tasks;
using HealthCheckSample.Controllers;
using HealthCheckSample.Models;

namespace UserHealthCheck
{
    //此頁為自訂檢查邏輯的頁面需實作介面 =>  public class UserApiHealthCheck : IHealthCheck{}
    public class UserApiHealthCheck : IHealthCheck
    {
        private readonly UserController _usersController;

        public UserApiHealthCheck(UserController usersController)
        {
            _usersController = usersController;
        }

        public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
        {
            try
            {
                // 在這裡添加您的 User 控制器 API 的健康檢查邏輯
                // 檢查您的 API 是否正常運作，並根據檢查結果返回適當的 HealthCheckResult

                // 假設您的 API 健康檢查邏輯為向 User 控制器的 Get 方法發出請求，
                // 如果請求成功且返回正確的結果，則將其視為健康，否則視為不健康

                var result = await _usersController.GetAsync();
                if (result != null)
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
