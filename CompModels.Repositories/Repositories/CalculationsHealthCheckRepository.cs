using CompModels.DAL.Models;
using CompModels.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebStruct.Shared;

namespace CompModels.Repositories.Repositories
{
    public class CalculationsHealthCheckRepository : ICalculationsHealthCheckRepository
    {
        private readonly GenStructContext _pgContext;

        public CalculationsHealthCheckRepository(string pgConnectionString)
        {
            _pgContext = ContextHelpers.GetContext(pgConnectionString);
        }

        public async Task<Dictionary<string, long[]>> GetFailedCalculationsAsync()
        {
            var result = new Dictionary<string, long[]>();

            var bezierFailed = await _pgContext.BezierCalculationRequests
                               .Where
                               (x => x.RequestStatusId == (short)RequestStatusesEnum.Error)
                               .Select
                               (x => x.Id)
                               .ToArrayAsync();

            if (bezierFailed.Length > 0)
                result.Add("Bezier", bezierFailed);

            return result;
        }
    }
}
