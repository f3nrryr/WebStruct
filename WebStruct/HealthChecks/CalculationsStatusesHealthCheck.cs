using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace WebStruct.HealthChecks
{
    public class CalculationsStatusesHealthCheck : IHealthCheck
    {
        public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
        {
            try
            {
                return HealthCheckResult.Healthy();
            }
            catch (Exception ex)
            {
                return HealthCheckResult.Unhealthy(ex.ToString());
            }
        }
    }
}
