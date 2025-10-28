using CompModels.DAL.Models;
using CompModels.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
    }
}
