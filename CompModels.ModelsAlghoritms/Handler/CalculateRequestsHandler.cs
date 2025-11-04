using CompModels.ModelsAlghoritms.ProxiesCodeCallers;
using CompModels.Repositories.Repositories;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using WebStruct.Shared;

namespace CompModels.ModelsAlghoritms.Handler
{
    /// <summary>
    /// Чувак, который будет создавать класс нужной модели в зависимости от указанного в б/д для calcRequestId типа модели и запускать расчет.
    /// </summary>
    public class CalculateRequestsHandler : ICalculateRequestsHandler
    {
        private readonly DbConnectionOptions _dbConnectionOptions;
        private readonly ILogger<CalculateRequestsHandler> _logger;

        public CalculateRequestsHandler(IOptions<DbConnectionOptions> dbConnectionOptions, ILogger<CalculateRequestsHandler> logger)
        {
            _dbConnectionOptions = dbConnectionOptions.Value;
            _logger = logger;
        }

        //Этот метод будет вызывать все модели по очереди. А его вызывать будет Worker HostedService.
        public async Task HandleAsync()
        {
            await Task.Yield();

            var bezierRepository = new BezierRepository(_dbConnectionOptions.Postgres);
            long bezierRequestIdForCalc = -1;
            try
            {
                bezierRequestIdForCalc = await bezierRepository.GetRequestIdToHandleChronologicAsync();
                if (bezierRequestIdForCalc != 0)
                {
                    _logger.LogInformation($"Начата обработка запроса на расчёт {bezierRequestIdForCalc}");

                    var bezierAlgorithm = new BezierAlgorithm(_dbConnectionOptions.Postgres);
                    await bezierAlgorithm.CalculateAsync(bezierRequestIdForCalc);
                }

                //var dlcaRepository... etc...
            }
            catch (Exception ex)
            {
                await bezierRepository.SetRequestFailedAsync(bezierRequestIdForCalc);
                _logger.LogError($"CalculationRequestId: {bezierRequestIdForCalc}. {ex}");
            }
        }
    }
}
