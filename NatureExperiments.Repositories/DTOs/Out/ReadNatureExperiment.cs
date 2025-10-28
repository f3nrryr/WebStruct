using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebStruct.Shared;

namespace NatureExperiments.Repositories.DTOs.Out
{
    public class ReadNatureExperiment
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Desciption { get; set; }
        public int CreatedBy { get; set; }
        public string LastUpdatedBy { get; set; }

        public List<PhysicalFile> Attachments { get; set; }
    }
}
