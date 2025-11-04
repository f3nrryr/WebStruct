using CompModels.CRUD.Services.Interfaces;
using CompModels.Repositories.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using WebStruct.Contracts;
using WebStruct.Contracts.ComputationalExperiments;
using WebStruct.Shared;

namespace WebStruct.Controllers.Calcs
{
    /// <summary>
    /// Создать заявку на расчёт. Получить статус заявки на расчёт. Получить результат готового расчёта. КОНКРЕТНЫЕ расчёты.
    /// </summary>
    [Route("api/v1/[controller]/")]
    [ProducesResponseType(typeof(ErrorsDetailResult), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(string), StatusCodes.Status404NotFound)]
    [Authorize]
    public class ComputationalExperimentsController : ControllerBase
    {
        private readonly ICompExperimentsDataStorageFacade _dataStorageFacadeService;
        private readonly DbConnectionOptions _dbConnectionOptions;

        public ComputationalExperimentsController(ICompExperimentsDataStorageFacade dataStorageFacadeService,
            IOptions<DbConnectionOptions> dbConnectionOptions)
        {
            _dataStorageFacadeService = dataStorageFacadeService;
            _dbConnectionOptions = dbConnectionOptions.Value;
        }

        [HttpGet("IsCalculationResultReady/{id}")]
        [ProducesResponseType(typeof(bool), StatusCodes.Status200OK)]
        public async Task<IActionResult> 
            IsCalculationResultReady
            ([FromHeader] Guid traceId, 
            [FromQuery] int compModelId, [FromRoute] int calculationRequestId,
            [FromQuery] int userRequesterId)
        {
            var isCalculationReady = await _dataStorageFacadeService
                                     .GetCalculationRequestStatusIdAsync
                                     (compModelId, calculationRequestId, userRequesterId)
                                     
                                     ==
                                     
                                     (short)RequestStatusesEnum.Ready
                                     
                                     ;

            return Ok(isCalculationReady);
        }

        /// <summary>
        /// Получить результат готового расчёта. Сперва вызови метод проверки готовности.
        /// </summary>
        /// <param name="traceId">Идентификатор для отслеживания пути запроса.</param>
        /// <remarks>
        /// Be careful.
        /// </remarks>
        /// <returns><see cref="string"/></returns>
        /// <response code="401">You have to auth via Login endpoint of AuthController.</response>
        /// <response code="404">Pay more attention. Not Found:)</response>
        /// <response code="400">You provided invalid value by some way... Most likely it was empty or null.</response> 
        /// <response code="500">Most likely, error on API server-side.</response>
        /// <exception cref="InvalidDataException"></exception>
        [HttpGet("GetCalculationResult/{calculationId}")]
        [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
        public async Task<IActionResult> 
            GetCalculationResult
            ([FromHeader] Guid traceId,
             [FromQuery] int compModelId, [FromRoute] int calculationRequestId,
             [FromQuery] int userRequesterId
            )
        {
            var modelResultString = await _dataStorageFacadeService
                                    .GetFinishedCalculationOutputAsync
                                    (compModelId, calculationRequestId, userRequesterId);
            if (!string.IsNullOrWhiteSpace(modelResultString))
                return Ok(modelResultString);

            return BadRequest(new ErrorsDetailResult
            {
                Errors = new string[] { "Расчет не найден" }
            });
        }

        [HttpPost()]
        [ProducesResponseType(typeof(long), StatusCodes.Status200OK)]
        public async Task<IActionResult> 
            RequestCalculation
            ([FromHeader] Guid traceId, 
            RequestCalculation request)
        {
            try
            {
                var createdCalcRequestId = await _dataStorageFacadeService
                                           .AddCalculationRequestAsync
                                           (request.ModelId, request.InputParamsValuesJSON);

                return Ok(createdCalcRequestId);
            }
            catch (UsefulException ex)
            {
                return BadRequest(new ErrorsDetailResult
                {
                    Title = ex.Message,
                    Errors = ex.Errors
                });
            }
        }

        [HttpGet("Statuses")]
        [ProducesResponseType(typeof(Dictionary<short, string>), StatusCodes.Status200OK)]
        public async Task<IActionResult>
            GetStatuses
            ([FromHeader] Guid traceId)
        {
            //TODO: cold start for handbooks. scripts or orm code-first.
            var requestStatusesRepository = new RequestStatusesRepository(_dbConnectionOptions.PG_ConnectionString);

            return Ok(await requestStatusesRepository.GetAsync());
        }
    }
}
