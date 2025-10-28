using CompModels.ModelsAlghoritms.Interfaces;
using CompModels.Repositories.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CompModels.ModelsAlghoritms.ProxiesCodeCallers
{
    //ТУТ ВЫЗОВЫ ИЗ NUGET-пакета с кодом моделей. или не nuget... как ето отлаживать тогда...
    //GenStruct.Algorithms = алгоритмы + их вход-выход. И консольный тестер. 1 комп. модель = 1 проект. И 1 нугет.
    internal class BezierAlgorithm : IAlgorithm
    {
        private readonly string _pgConnectionString;

        public BezierAlgorithm(string pgConnectionString)
        {
            _pgConnectionString = pgConnectionString;
        }

        public async Task CalculateAsync(long calculationRequestId)
        {
            var bezierRepository = new BezierRepository(_pgConnectionString);
            var calculationInputParams = await bezierRepository.GetRequestInputDataForCalculateAsyncById(calculationRequestId);

            //CPU-bound operation for thread from thread-pool:
            var result = await Task.Run(() =>
            {
                return "1"; // TODO: вызов класса и метода Calculate модели из Nuget.
            });

            //await bezierRepository
            //      .FinishCalculationRequestAsync
            //      (calculationRequestId, result.CalcedPoroisty, result.PhysicalFile);
        }
    }
}
