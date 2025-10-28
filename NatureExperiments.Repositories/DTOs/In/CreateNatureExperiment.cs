using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebStruct.Shared;

namespace NatureExperiments.Repositories.DTOs.In
{
    public class CreateNatureExperiment
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public int CreatedBy { get; set; }
        public List<PhysicalFile> Attachments { get; set; }
    }
}
