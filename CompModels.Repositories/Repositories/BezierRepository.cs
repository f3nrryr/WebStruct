using CompModels.Repositories.DTOs.In;
using CompModels.Repositories.DTOs.Out;
using CompModels.Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CompModels.Repositories.Repositories
{
    public class BezierRepository : IBezierRepository
    {
        private readonly context... _pgContext;

        public BezierRepository(string pgConnectionString)
        {

        }

        public int AddCalculationRequest(BezierInputParamsValues input)
        {

        }

        public Task<int> GetCalculationRequestStatusIdAsync
            (int compModelId, int calculationRequestId, int userRequesterId)
        {

        }

        public Task<string> GetFinishedCalculationOutputAsync
            (int compModelId, int calculationRequestId, int userRequesterId)
        {

        }
    }
}
