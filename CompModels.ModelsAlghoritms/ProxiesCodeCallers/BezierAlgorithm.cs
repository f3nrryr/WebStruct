using CompModels.ModelsAlghoritms.Interfaces;
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
        public Task CalculateAsync(int calculationRequestId)
        {
            //todo: proxy call... thread from threadpool?
            throw new NotImplementedException();
            //todo: db add result info... metadata + files + output params.
        }
    }
}
