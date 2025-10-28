using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CompModels.Repositories.Interfaces
{
    public interface IRequestStatusesRepository
    {
        Task<Dictionary<short, string>> GetAsync();
    }
}
