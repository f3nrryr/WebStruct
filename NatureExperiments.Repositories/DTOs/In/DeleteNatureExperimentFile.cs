using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NatureExperiments.Repositories.DTOs.In
{
    public class DeleteNatureExperimentFile
    {
        public long ExperimentId { get; set; }
        public long FileId { get; set; }
        public int UserRequesterId { get; set; }
    }
}
