using System;
using System.Collections.Generic;
using System.Text;

namespace WebStruct.Admin.Contracts.ModelsAlgorithms
{
    public class CreateModelAlgorithmRequest
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public bool IsCellularAutomaton { get; set; }
        public bool IsPorousModel { get; set; }
        public Guid CreatedBy { get; set; }
        public string InputParamsJsonExample { get; set; }
        public string OutputParamsJsonExample { get; set; }
    }
}
