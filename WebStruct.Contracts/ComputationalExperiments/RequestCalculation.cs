using System;
using System.Collections.Generic;
using System.Text;

namespace WebStruct.Contracts.ComputationalExperiments
{
    public class RequestCalculation
    {
        public int ModelId { get; set; }

        /// <summary>
        /// ADR: здесь могут быть разные модели.
        /// Десериализация и валидация на слое BLL (DataStorageFacadeService).
        /// В зависимости от ModelId. UserRequesterId здесь же.
        /// </summary>
        public string InputParamsValuesJSON { get; set; }
    }
}
