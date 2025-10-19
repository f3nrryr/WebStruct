using Microsoft.AspNetCore.Mvc;
using WebStruct.Contracts.ComputationalExperiments;

namespace WebStruct.Controllers.Calcs
{
    /// <summary>
    /// Создать заявку на расчёт. Получить статус заявки на расчёт. Получить результат готового расчёта. КОНКРЕТНЫЕ расчёты.
    /// </summary>
    [Route("api/v1/[controller]/")]
    [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(string), StatusCodes.Status404NotFound)]
    public class ComputationalExperimentsController : ControllerBase
    {
        public ComputationalExperimentsController()
        {
            //todo: ilogger, services.
        }

        [HttpGet("IsCalculationResultReady/{id}")]
        [ProducesResponseType(typeof(bool), StatusCodes.Status200OK)]
        public async Task<IActionResult> 
            IsCalculationResultReady
            ([FromHeader] Guid traceId, 
            [FromQuery] int id)
        {
            //bool
            return Ok();
        }

        /// <summary>
        /// Получить результат готового расчёта. Сперва вызови метод проверки готовности..
        /// </summary>
        /// <param name="traceId">Идентификатор для отслеживания пути запроса.</param>
        /// <remarks>
        /// Be careful.
        /// </remarks>
        /// <returns><see cref="GetCalculationResult"/></returns>
        /// <response code="401">You have to auth via Login endpoint of AuthController.</response>
        /// <response code="404">Pay more attention. Not Found:)</response>
        /// <response code="400">You provided invalid value by some way... Most likely it was empty or null.</response> 
        /// <response code="500">Most likely, error on API server-side.</response>
        /// <exception cref="InvalidDataException"></exception>
        [HttpGet("GetCalculationResult/{calculationId}")]
        [ProducesResponseType(typeof(GetCalculationResult), StatusCodes.Status200OK)]
        public async Task<IActionResult> 
            GetCalculationResult
            ([FromHeader] Guid traceId,
             [FromRoute] int calculationId,
             [FromQuery] int userRequesterId
            )
        {
            //TODO filter by user requester id.

            var g = new GetCalculationResult();
            return Ok(); 
        }

        [HttpPost()]
        public async Task<IActionResult> 
            RequestCalculation
            ([FromHeader] Guid traceId, 
            RequestCalculation request)
        {
            //return int calculationRequestId.
        }

        [HttpGet("Statuses")]
        public async Task<IActionResult>
            GetStatuses
            ([FromHeader] Guid traceId)
        {
            //todo: statuses handbook. 1234.
            return;
        }
    }
}
