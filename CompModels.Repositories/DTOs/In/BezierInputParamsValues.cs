using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CompModels.Repositories.DTOs.In
{
    public class BezierInputParamsValues : InputParamsValuesBase
    {
        public float DesiredPorosity { get; set; }
        public int FibreDiameter { get; set; }
    }
}
