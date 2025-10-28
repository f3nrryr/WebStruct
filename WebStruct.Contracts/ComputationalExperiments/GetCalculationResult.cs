using System;
using System.Collections.Generic;
using System.Text;

namespace WebStruct.Contracts.ComputationalExperiments
{
    /// <summary>
    /// Универсальный ответ API с результатом расчёта и его входными параметрами.
    /// </summary>
    public class GetCalculationResult
    {
        //Дублируем имя модели, чтобы избежать лишних запросов по API при просмотре эксперимента.
        public string ModelName { get; set; }

        public int StatusId { get; set; } // 1 = new. 2 = inWork. 3 = ok. 4 = bad.

        public string InputParamsValuesJSON { get; set; }
        public string OutputParamsValuesJSON { get; set; }
    }
}
