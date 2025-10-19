using Microsoft.EntityFrameworkCore;
using NatureExperiments.DAL.Models;
using NatureExperiments.Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NatureExperiments.Repositories.Repositories
{
    public class NatureExperimentsRepository : INatureExperimentsRepository
    {
        private readonly GenStructContext _context;

        public NatureExperimentsRepository(string connectionString)
        {
            var contextOptions = new DbContextOptionsBuilder<GenStructContext>()
                                 .UseNpgsql(connectionString)
                                 .Options;

            _context = new GenStructContext(contextOptions);
        }

        public Task<> 
    }
}
