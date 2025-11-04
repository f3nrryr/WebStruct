using CompModels.CRUD.Services.Interfaces;
using CompModels.CRUD.Services.Validators;
using CompModels.Repositories.DTOs.In.Bezier;
using CompModels.Repositories.Interfaces;
using CompModels.Repositories.Repositories;
using CompModels.Shared.Common.Enums;
using Microsoft.Extensions.Options;
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
    public class CompExperimentsDataStorageFacade : ICompExperimentsDataStorageFacade
    {
        private readonly DbConnectionOptions _dbConnections;

        public CompExperimentsDataStorageFacade(IOptions<DbConnectionOptions> dbConnections)
        {
            _dbConnections = dbConnections.Value;
        }

        public async Task<long> 
            AddCalculationRequestAsync
            (int compModelId, string inputJSON)
        {
            //todo: валидация типа не отриц параметры и т.п. иначе custom exception.

            long calculationRequestId = -1;

            switch ((AlgorithmsEnum)compModelId)
            {
                case AlgorithmsEnum.Bezier:
                    var bezierModelInput = JsonSerializer.Deserialize<BezierInputParamsValues>(inputJSON);
                    var bezierValidator = new BezierValidator();

                    var errors = bezierValidator.Validate(bezierModelInput).ToArray();
                    if (errors.Length > 0)
                        throw new UsefulException(HttpStatusCode.BadRequest, errors);

                    calculationRequestId = new BezierRepository(_dbConnections.Postgres).AddCalculationRequest(bezierModelInput);

                    break;

                default:
                    break;
            }

            return calculationRequestId;
        }

        public async Task<short> 
            GetCalculationRequestStatusIdAsync
            (int compModelId, int calculationRequestId, int userRequesterId)
        {
            short calculationRequestStatusId = 0;

            switch ((AlgorithmsEnum)compModelId)
            {
                case AlgorithmsEnum.Bezier:

                    calculationRequestId = await new BezierRepository(_dbConnections.Postgres)
                                           .GetCalculationRequestStatusIdAsync
                                           (calculationRequestId, userRequesterId);

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
            string resultInputAndOutputJSON = string.Empty;

            var modelsAlgorithmsRepository = new ModelsAlgorithmsRepository(_dbConnections.Postgres);
            var modelNameById = await modelsAlgorithmsRepository.GetModelNameByIdAsync(compModelId);
            switch ((AlgorithmsEnum)compModelId)
            {
                case AlgorithmsEnum.Bezier:

                    var bezierInputOutput = await new BezierRepository(_dbConnections.Postgres)
                                            .GetFinishedCalculationOutputAsync
                                            (calculationRequestId, userRequesterId);

                    resultInputAndOutputJSON = JsonSerializer.Serialize
                                               (new
                                               {
                                                   ModelName = modelNameById,
                                                   RequestStatusId = bezierInputOutput.RequestStatusId,
                                                   InputParamsJSON = bezierInputOutput.InputParamsValues,
                                                   OutputParamsJSON = bezierInputOutput.OutputParamsValues
                                               });

                    break;

                default:
                    break;
            }

            return resultInputAndOutputJSON;
        }
    }
}
