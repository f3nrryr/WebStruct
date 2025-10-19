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
        long AddCalculationRequest(BezierInputParamsValues input);
        Task<short> GetCalculationRequestStatusIdAsync
            (int calculationRequestId, int userRequesterId);
        Task<string> GetFinishedCalculationOutputAsync
            (int calculationRequestId, int userRequesterId);
    }
}
