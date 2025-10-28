using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebStruct.Shared;

namespace NatureExperiments.Repositories.DTOs.In
{
    public class UpdateNatureExperiment
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public int LastUpdatedBy { get; set; }
    }
}
