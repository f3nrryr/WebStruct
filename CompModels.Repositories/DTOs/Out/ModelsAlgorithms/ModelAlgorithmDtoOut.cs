using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CompModels.Repositories.DTOs.Out.ModelsAlgorithms
{
    public class ModelAlgorithmDtoOut
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public bool IsPorousModel { get; set; }
        public bool IsCellularAutomaton { get; set; }
        public DateTime CreatedAt { get; set; }
        public Guid CreatedBy { get; set; }
        public string InputParamsJsonExample { get; set; }
        public string OutputParamsJsonExample { get; set; }
    }
}
