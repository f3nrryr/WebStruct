using CompModels.DAL.Models;
using CompModels.Repositories.DTOs.In.ModelsAlgorhitms;
using CompModels.Repositories.DTOs.Out.ModelsAlgorithms;
using CompModels.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Net;
using WebStruct.Shared;

namespace CompModels.Repositories.Repositories
{
    public class ModelsAlgorithmsRepository : IModelsAlgorithmsRepository
    {
        private readonly GenStructContext _pgContext;

        public ModelsAlgorithmsRepository(string pgConnectionString)
        {
            _pgContext = ContextHelpers.GetContext(pgConnectionString);
        }

        public Task<string?> GetModelNameByIdAsync(int compModelId)
        {
            return _pgContext.ComputationalModels
                   .Where(x => x.Id == compModelId)
                   .Select(x => x.Name)
                   .SingleOrDefaultAsync();
        }

        //UPDATE НЕ БУДЕТ. НОВУЮ МОДЕЛЬ СОЗДАВАТЬ ЕСЛИ ЧЕ... А СТАРУЮ УДАЛЯТЬ. ВОТ ТАК. Ибо странно МЕНЯТЬ... СТРАННО.

        public Task<List<ModelAlgorithmDtoOut>> GetModelsAlgorithmsHandbookAsync()
        {
            return _pgContext.ComputationalModels
                   .Select(x => new ModelAlgorithmDtoOut
                   {
                       Id = x.Id,
                       CreatedAt = x.CreatedAt,
                       CreatedBy = x.CreatedBy,
                       InputParamsJsonExample = x.InputParamsJsonExample,
                       IsCellularAutomaton = x.IsCellularAutomaton,
                       IsPorousModel = x.IsPorousModel,
                       Name = x.Name,
                       OutputParamsJsonExample = x.OutputParamsJsonExample,
                       Description = x.Description
                   })
                   .ToListAsync();
        }

        public int CreateModelAlgorithm(CreateModelAlgorhitm createModelAlgorhitm)
        {
            var res = _pgContext.ComputationalModels
                       .Add(new ComputationalModel
                       {
                           InputParamsJsonExample = createModelAlgorhitm.InputParamsJsonExample,
                           CreatedAt = DateTime.Now,
                           CreatedBy = createModelAlgorhitm.CreatedBy,
                           IsCellularAutomaton = createModelAlgorhitm.IsCellularAutomaton,
                           IsPorousModel = createModelAlgorhitm.IsPorousModel,
                           Name = createModelAlgorhitm.Name,
                           OutputParamsJsonExample = createModelAlgorhitm.OutputParamsJsonExample,
                           Description = createModelAlgorhitm.Description
                       });

            _pgContext.SaveChanges();

            return res.Entity.Id;
        }

        public async Task DeleteModelAlgorithmAsync(int modelAlgorithmId)
        {
            var removinModelAlgorithm = await _pgContext.ComputationalModels
                                        .Where(x => x.Id == modelAlgorithmId)
                                        .SingleOrDefaultAsync();

            if (removinModelAlgorithm == null)
                throw new UsefulException(HttpStatusCode.NotFound, new string[] { "Не найдена модель для удаления" });

            _pgContext.ComputationalModels.Remove(removinModelAlgorithm);

            await _pgContext.SaveChangesAsync();
        }
    }
}
