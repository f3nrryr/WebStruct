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
    public class RequestStatusesRepository : IRequestStatusesRepository
    {
        private readonly GenStructContext _pgContext;

        public RequestStatusesRepository(string pgConnectionString)
        {
            _pgContext = ContextHelpers.GetContext(pgConnectionString);
        }

        public Task<Dictionary<short, string>> GetAsync()
        {
            return _pgContext
                   .RequestsStatusesHandbooks
                   .ToDictionaryAsync
                   (k => k.Id, v => v.Name);
        }
    }
}
