using CompModels.DAL.Models;
using CompModels.Repositories.DTOs.In.Bezier;
using CompModels.Repositories.DTOs.Out.Bezier;
using CompModels.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using WebStruct.Shared;
namespace CompModels.Repositories.Repositories
{
    public class BezierRepository : IBezierRepository
    {
        private readonly GenStructContext _pgContext;

        public BezierRepository(string pgConnectionString)
        {
            _pgContext = ContextHelpers.GetContext(pgConnectionString);
        }

        public Task<long> GetRequestIdToHandleChronologicAsync()
        {
            return _pgContext
                   .BezierCalculationRequests
                   .Where
                   (x => x.RequestStatusId == (short)RequestStatusesEnum.New)
                   .OrderBy
                   (x => x.Id)
                   .Select
                   (x => x.Id
                   )
                   .FirstOrDefaultAsync();
        }

        public Task<BezierInputParamsValues?> GetRequestInputDataForCalculateAsyncById(long requestId)
        {
            return _pgContext
                   .BezierCalculationRequests
                   .Where
                   (x => x.Id == requestId)
                   .Select
                   (x => new BezierInputParamsValues
                   {
                       X = x.X,
                       Y = x.Y,
                       Z = x.Z,

                       FibreDiameter = (int)x.FibreDiameter,
                       DesiredPorosity = x.DesiredPorosity
                   }
                   )
                   .FirstOrDefaultAsync();    
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
                RequestedAt = DateTime.Now,
                RequestStatusId = (short)RequestStatusesEnum.New
            });

            _pgContext.SaveChanges();

            return res.Entity.Id;
        }

        public Task<short> GetCalculationRequestStatusIdAsync
            (int calculationRequestId, int? userRequesterId)
        {
            var baseQuery = _pgContext.BezierCalculationRequests
                            .Where(x => x.Id == calculationRequestId);

            if (userRequesterId.HasValue)
                baseQuery = baseQuery
                            .Where
                            (x => x.UserRequesterId == userRequesterId.Value);

            return baseQuery
                   .Select(x => x.RequestStatusId)
                   ?.SingleOrDefaultAsync();
        }

        public async Task<BezierResponse> GetFinishedCalculationOutputAsync
            (int calculationRequestId, int? userRequesterId)
        {
            var baseQuery = _pgContext.BezierCalculationRequests
                            .Where(x => x.Id == calculationRequestId
                                        &&
                                        x.RequestStatusId == (short)RequestStatusesEnum.Success
                            );

            if (userRequesterId.HasValue)
                baseQuery = baseQuery.Where(x => x.UserRequesterId == userRequesterId.Value);

            BezierResponse res = await baseQuery
                                 .Select(x => new BezierResponse
                                 {
                                     InputParamsValues = new BezierInputParamsValues
                                     {
                                         X = x.X,
                                         Y = x.Y,
                                         Z = x.Z,

                                         DesiredPorosity = x.DesiredPorosity,

                                         FibreDiameter = (int)x.FibreDiameter,

                                         UserRequesterId = x.UserRequesterId
                                     },
                                     OutputParamsValues = new BezierOutputParamsValues
                                     {
                                         CalcResultPorosity = (float)x.CalcResultPorosity
                                     }
                                 }
                                 )
                                 .SingleOrDefaultAsync();

            return res;
        }

        public async Task FinishCalculationRequestAsync(long calculationRequestId, float calcedPorosity, PhysicalFile resultField)
        {
            var calcRequest = await _pgContext.BezierCalculationRequests
                              .Where
                              (x => x.Id == calculationRequestId)
                              .SingleOrDefaultAsync();
            if (calcRequest == null)
                //todo??? custom ex???
                throw new Exception($"ЗАПРОС НЕ НАЙДЕН: {calculationRequestId}");

            calcRequest.CalcResultPorosity = calcedPorosity;
            calcRequest.RequestStatusId = (short)RequestStatusesEnum.Success;

            var file = _pgContext
                       .BezierResultsPhysicalFiles
                       .Add(new BezierResultsPhysicalFile
                       {
                           FileNameWithoutDotAndExtension = resultField.FileNameWithoutDotAndExtension,
                           Extension = resultField.Extension,
                           FileContent = resultField.FileContent
                       });

            var requestFileBind = _pgContext
                                  .BezierRequestFiles
                                  .Add(new BezierRequestFile
                                  {
                                      ExperimentId = calculationRequestId,
                                      File = file.Entity
                                  });

            await _pgContext.SaveChangesAsync();
        }

        public async Task SetRequestFailedAsync(long calculationRequestId)
        {
            var calcRequest = await _pgContext.BezierCalculationRequests
                              .Where
                              (x => x.Id == calculationRequestId)
                              .SingleOrDefaultAsync();
            if (calcRequest == null)
                //todo??? custom ex???
                throw new Exception($"ЗАПРОС НЕ НАЙДЕН: {calculationRequestId}");

            calcRequest.RequestStatusId = (short)RequestStatusesEnum.Error;

            await _pgContext.SaveChangesAsync();
        }
    }
}
