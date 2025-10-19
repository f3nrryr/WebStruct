using CompModels.CRUD.Services.Interfaces;
using CompModels.Repositories.DTOs.In;
using CompModels.Repositories.Interfaces;
using CompModels.Repositories.Repositories;
using CompModels.Shared.Common.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using WebStruct.Shared;

namespace CompModels.CRUD.Services.Services
{
    /// <summary>
    /// Контроллер комп. эксп. вызывает его. Десериализует строку в параметры указанной модели и добавляет или читает данные о расчёте.
    /// </summary>
    public class DataStorageFacadeService : IDataStorageFacadeService
    {
        private readonly DbConnectionOptions _dbConnections;

        public DataStorageFacadeService(IOptions<DbConnectionOptions> dbConnections)
        {
            _dbConnections = dbConnections.Value;
        }

        public async Task<int> 
            AddCalculationRequestAsync
            (int compModelId, string inputJSON)
        {
            //todo: валидация типа не отриц параметры и т.п. иначе custom exception.

            int calculationRequestId = -1;

            switch ((AlgorithmsEnum)compModelId)
            {
                case AlgorithmsEnum.Bezier:
                    var bezierModelInput = JsonSerializer.Deserialize<BezierInputParamsValues>(inputJSON);
                    if (bezierModelInput == null || bezierModelInput?.X == null)
                        throw new UsefulException(HttpStatusCode.BadRequest, "Плохой запрос...");

                    calculationRequestId = new BezierRepository().AddCalculationRequest(bezierModelInput);

                    break;

                default:
                    break;
            }

            return calculationRequestId;
        }

        public async Task<int> 
            GetCalculationRequestStatusIdAsync
            (int compModelId, int calculationRequestId, int userRequesterId)
        {
            int calculationRequestStatusId = -1;

            switch ((AlgorithmsEnum)compModelId)
            {
                case AlgorithmsEnum.Bezier:

                    calculationRequestId = new BezierRepository().();

                    break;

                default:
                    break;
            }

            return calculationRequestStatusId;
        }

        public async Task<string> 
            GetFinishedCalculationOutputAsync
            (int compModelId, int calculationRequestId, int userRequesterId)
        {
            string compExpOutputJSON = string.Empty;

            switch ((AlgorithmsEnum)compModelId)
            {
                case AlgorithmsEnum.Bezier:

                    calculationRequestId = new BezierRepository().();

                    break;

                default:
                    break;
            }

            return compExpOutputJSON;
        }
    }
}
