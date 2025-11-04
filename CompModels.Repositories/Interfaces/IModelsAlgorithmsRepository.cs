using CompModels.Repositories.DTOs.In.ModelsAlgorhitms;
using CompModels.Repositories.DTOs.Out.ModelsAlgorithms;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CompModels.Repositories.Interfaces
{
    public interface IModelsAlgorithmsRepository
    {
        Task<string?> GetModelNameByIdAsync(int compModelId);
        Task<List<ModelAlgorithmDtoOut>> GetModelsAlgorithmsHandbookAsync();
        Task DeleteModelAlgorithmAsync(int modelAlgorithmId);
        int CreateModelAlgorithm(CreateModelAlgorhitm dto);
    }
}
