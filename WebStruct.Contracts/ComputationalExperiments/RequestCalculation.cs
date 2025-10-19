using System;
using System.Collections.Generic;
using System.Text;

namespace WebStruct.Contracts.ComputationalExperiments
{
    public class RequestCalculation
    {
        public int UserRequesterId { get; set; }
        public int ModelId { get; set; }
        public string InputParamsValuesJSON { get; set; }
    }
}
