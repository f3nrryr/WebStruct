using CompModels.ModelsAlghoritms.ProxiesCodeCallers;
using CompModels.Repositories.Repositories;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebStruct.Shared;

namespace CompModels.ModelsAlghoritms.Handler
{
    /// <summary>
    /// Чувак, который будет создавать класс нужной модели в зависимости от указанного в б/д для calcRequestId типа модели и запускать расчет.
    /// </summary>
    public class CalculateRequestsHandler : ICalculateRequestsHandler
    {
        private readonly DbConnectionOptions _dbConnectionOptions;

        public CalculateRequestsHandler(IOptions<DbConnectionOptions> dbConnectionOptions)
        {
            _dbConnectionOptions = dbConnectionOptions.Value;
        }

        //Этот метод будет вызывать все модели по очереди. А его вызывать будет Worker HostedService.
        public async Task HandleAsync()
        {
            var bezierRepository = new BezierRepository(_dbConnectionOptions.PG_ConnectionString);
            var bezierRequestIdForCalc = await bezierRepository.GetRequestIdToHandleChronologicAsync();
            if (bezierRequestIdForCalc != 0)
            {
                var bezierAlgorithm = new BezierAlgorithm(_dbConnectionOptions.PG_ConnectionString);
                await bezierAlgorithm.CalculateAsync(bezierRequestIdForCalc);
            }

            //var dlcaRepository... etc...
        }
    }
}
