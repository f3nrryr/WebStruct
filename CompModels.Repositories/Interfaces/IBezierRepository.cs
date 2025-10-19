using CompModels.Repositories.DTOs.In;
using CompModels.Repositories.DTOs.Out;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CompModels.Repositories.Interfaces
{
    public interface IBezierRepository
    {
        int AddCalculationRequest(BezierInputParamsValues input);
    }
}
