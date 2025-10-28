using System;
using System.Collections.Generic;
using System.Text;

namespace WebStruct.Admin.Contracts.ModelsAlgorithms
{
    public class CreateOrUpdateModelAlgorithmRequest
    {
        public string Name { get; set; }
        public string InputParamsJSON { get; set; }
        public string OutputParamsJSON { get; set; }
    }
}
