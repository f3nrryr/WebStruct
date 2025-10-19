using CompModels.DAL.Models;
using CompModels.Repositories.DTOs.In;
using CompModels.Repositories.DTOs.Out;
using CompModels.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
namespace CompModels.Repositories.Repositories
{
    public class BezierRepository : IBezierRepository
    {
        private readonly GenStructContext _pgContext;

        public BezierRepository(string pgConnectionString)
        {
            _pgContext = ContextHelpers.GetContext(pgConnectionString);
        }

        public long AddCalculationRequest(BezierInputParamsValues input)
        {
            var res = _pgContext.BezierCalculationRequests.Add(new BezierCalculationRequest
            {
                X = input.X,
                Y = input.Y,
                Z = input.Z,

                DesiredPorosity = input.DesiredPorosity,
                FibreDiameter = input.FibreDiameter,

                UserRequesterId = input.UserRequesterId,
                RequestedAt = input.RequestedAt,
                RequestStatusId = 1
            });

            _pgContext.SaveChanges();

            return res.Entity.Id;
        }

        public Task<short> GetCalculationRequestStatusIdAsync
            (int calculationRequestId, int userRequesterId)
        {
            return _pgContext.BezierCalculationRequests
                   .Where(x => x.Id == calculationRequestId
                   && x.UserRequesterId == userRequesterId)
                   .Select(x => x.RequestStatusId)
                   .SingleOrDefaultAsync();
        }

        public async Task<string> GetFinishedCalculationOutputAsync
            (int calculationRequestId, int userRequesterId)
        {
            BezierOutputParamsValues res = await _pgContext.BezierCalculationRequests
                                           .Where(x => x.Id == calculationRequestId
                                           && x.UserRequesterId == userRequesterId
                                           && x.RequestStatusId == 3
                                           )
                                           .Select(x => new BezierOutputParamsValues
                                           {
                                               CalcResultPorosity = (float)x.CalcResultPorosity
                                           })
                                           .SingleOrDefaultAsync();

            return JsonConvert.SerializeObject(res);
        }
    }
}
