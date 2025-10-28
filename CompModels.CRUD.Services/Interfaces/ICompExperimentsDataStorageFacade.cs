using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CompModels.CRUD.Services.Interfaces
{
    public interface ICompExperimentsDataStorageFacade
    {
        Task<long> AddCalculationRequestAsync(int compModelId, string inputJSON);
        Task<short> GetCalculationRequestStatusIdAsync(int compModelId, int calculationRequestId, int userRequesterId);
        Task<string> GetFinishedCalculationOutputAsync(int compModelId, int calculationRequestId, int userRequesterId);
    }
}
