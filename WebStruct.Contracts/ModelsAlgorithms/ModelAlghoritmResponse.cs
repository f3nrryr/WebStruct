using System;
using System.Collections.Generic;
using System.Text;

namespace WebStruct.Contracts.ModelsAlgorithms
{
    public class ModelAlgorithmResponse
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string InputParamsJsonExample { get; set; }
        public string OutputParamsJsonExample { get; set; }
        public bool IsPorousModel { get; set; }
        public bool IsCellularAutomaton { get; set; }
        public Guid CreatedBy { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
