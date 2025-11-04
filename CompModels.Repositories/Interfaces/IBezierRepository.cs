using CompModels.Repositories.DTOs.In.Bezier;
using CompModels.Repositories.DTOs.Out.Bezier;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebStruct.Shared;

namespace CompModels.Repositories.Interfaces
{
    public interface IBezierRepository
    {
        Task FinishCalculationRequestAsync(long calculationRequestId, float calcedPorosity, PhysicalFile resultField);
        Task<long> GetRequestIdToHandleChronologicAsync();
        Task<BezierInputParamsValues?> GetRequestInputDataForCalculateAsyncById(long requestId);

        long AddCalculationRequest(BezierInputParamsValues input);
        Task<short> GetCalculationRequestStatusIdAsync
            (int calculationRequestId, int? userRequesterId = null);
        Task<BezierResponse> GetFinishedCalculationOutputAsync
            (int calculationRequestId, int? userRequesterId = null);
    }
}
