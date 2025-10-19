using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CompModels.CRUD.Services.Interfaces
{
    public interface IDataStorageFacadeService
    {
        Task<int> AddCalculationRequestAsync(int compModelId, string inputJSON);
        Task<int> GetCalculationRequestStatusIdAsync(int compModelId, int calculationRequestId, int userRequesterId);
        Task<string> GetFinishedCalculationOutputAsync(int compModelId, int calculationRequestId, int userRequesterId);
    }
}
