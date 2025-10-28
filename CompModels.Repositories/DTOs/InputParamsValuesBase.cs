using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CompModels.Repositories.DTOs
{
    public class InputParamsValuesBase
    {
        public int X { get; set; }
        public int Y { get; set; }
        public int? Z { get; set; }

        public bool Is3D => Z.HasValue;



        public int UserRequesterId;
    }
}
