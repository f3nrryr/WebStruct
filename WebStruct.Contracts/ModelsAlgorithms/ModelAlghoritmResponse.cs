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
        public string InputParamsJSON { get; set; }
        public string OutputParamsJSON { get; set; }
    }
}
