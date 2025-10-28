using CompModels.Repositories.DTOs.In;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CompModels.Repositories.DTOs.Out
{
    public class BezierResponse
    {
        public BezierInputParamsValues InputParamsValues { get; set; }
        public BezierOutputParamsValues OutputParamsValues { get; set; }
        public int RequestStatusId { get; set; }
    }
}
