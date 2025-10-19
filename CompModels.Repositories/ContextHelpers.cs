using CompModels.DAL.Models;
using Microsoft.EntityFrameworkCore;

namespace CompModels.Repositories
{
    internal static class ContextHelpers
    {
        internal static GenStructContext GetContext(string pgConnectionString)
        {
            var contextOptions = new DbContextOptionsBuilder<GenStructContext>()
                                 .UseNpgsql(pgConnectionString)
                                 .Options;

            return new GenStructContext(contextOptions);
        }
    }
}
