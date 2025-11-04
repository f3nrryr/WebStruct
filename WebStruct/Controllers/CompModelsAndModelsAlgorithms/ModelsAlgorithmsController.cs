using CompModels.Repositories.Interfaces;
using CompModels.Repositories.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using WebStruct.Admin.Contracts.ModelsAlgorithms;
using WebStruct.Contracts;
using WebStruct.Contracts.ModelsAlgorithms;
using WebStruct.Shared;

namespace WebStruct.Controllers.CompModelsAndModelsAlgorithms
{
    /// <summary>
    /// Справка о комп. моделях. Их параметры и т.п. в json'ах для фронтенда. 
    /// + СОЗДАТЬ или УДАЛИТЬ комп. модель (по сути, всё это просто справочник для фронта).
    /// АПДЕЙТ НЕ ЗАПЛАНИРОВАН! НЕ ХОЧУ!))) Логичнее создавать и удалять. Для целостности данных.
    /// </summary>
    [Route("api/v1/[controller]/")]
    [ProducesResponseType(typeof(ErrorsDetailResult), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(string), StatusCodes.Status404NotFound)]
    [Authorize]
    public class ModelsAlgorithmsController : ControllerBase
    {
        private readonly IModelsAlgorithmsRepository _modelsAlgorithmsRepository;

        public ModelsAlgorithmsController(IOptions<DbConnectionOptions> options)
        {
            _modelsAlgorithmsRepository = new ModelsAlgorithmsRepository(options.Value.PG_ConnectionString);
        }

        [HttpGet("Handbook")]
        [ProducesResponseType(typeof(List<ModelAlgorithmResponse>), StatusCodes.Status200OK)]
        public async Task<IActionResult> Handbook()
        {
            try
            {
                return Ok
                       (
                        (await _modelsAlgorithmsRepository
                              .GetModelsAlgorithmsHandbookAsync()
                        )
                        .Select
                        (x => new ModelAlgorithmResponse
                        {
                            Id = x.Id,
                            Name = x.Name,
                            InputParamsJsonExample = x.InputParamsJsonExample,
                            OutputParamsJsonExample = x.OutputParamsJsonExample,
                            IsCellularAutomaton = x.IsCellularAutomaton,
                            IsPorousModel = x.IsPorousModel,
                            Description = x.Description,
                            CreatedBy = x.CreatedBy,
                            CreatedAt = x.CreatedAt
                        }
                        )
                        .ToList()
                       );
            }
            catch (Exception ex)
            {
                return BadRequest(new ErrorsDetailResult
                {
                    Errors = new string[] { ex.ToString() }
                });
            }
        }

        [HttpPost]
        [ProducesResponseType(typeof(int), StatusCodes.Status200OK)]
        [Authorize(Roles = "Admin")]
        public IActionResult Create(CreateModelAlgorithmRequest request)
        {
            try
            {
                return Ok
                       (
                        ( _modelsAlgorithmsRepository
                          .CreateModelAlgorithm(new CompModels.Repositories.DTOs.In.ModelsAlgorhitms.CreateModelAlgorhitm
                          {
                              CreatedBy = request.CreatedBy,
                              Description = request.Description,
                              InputParamsJsonExample = request.InputParamsJsonExample,
                              IsCellularAutomaton = request.IsCellularAutomaton,
                              IsPorousModel = request.IsPorousModel,
                              Name = request.Name,
                              OutputParamsJsonExample = request.OutputParamsJsonExample
                          })
                        )
                       );
            }
            catch (Exception ex)
            {
                return BadRequest(new ErrorsDetailResult
                {
                    Errors = new string[] { ex.ToString() }
                });
            }
        }

        [HttpDelete]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteAsync(int modelAlgorithmId)
        {
            try
            {
                await _modelsAlgorithmsRepository
                      .DeleteModelAlgorithmAsync(modelAlgorithmId);

                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(new ErrorsDetailResult
                {
                    Errors = new string[] { ex.ToString() }
                });
            }
        }
    }
}
