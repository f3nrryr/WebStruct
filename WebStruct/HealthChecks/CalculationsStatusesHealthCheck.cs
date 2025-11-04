using CompModels.Repositories.Interfaces;
using CompModels.Repositories.Repositories;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using WebStruct.Shared;

namespace WebStruct.HealthChecks
{
    public class CalculationsStatusesHealthCheck : IHealthCheck
    {
        private readonly DbConnectionOptions _options;

        public CalculationsStatusesHealthCheck(IOptions<DbConnectionOptions> options)
        {
            _options = options.Value;
        }

        public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
        {
            try
            {
                var failedCalculations = await new CalculationsHealthCheckRepository(_options.Postgres).GetFailedCalculationsAsync();

                if (failedCalculations.Count > 0)
                {
                    return HealthCheckResult.Unhealthy($"Упали расчёты: {JsonConvert.SerializeObject(failedCalculations)}");
                }

                return HealthCheckResult.Healthy("ВСЕ ПРЕКРАСНО)");
            }
            catch (Exception ex)
            {
                return HealthCheckResult.Unhealthy(ex.ToString());
            }
        }
    }
}
