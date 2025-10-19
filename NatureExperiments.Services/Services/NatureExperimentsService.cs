using NatureExperiments.Repositories.Interfaces;
using NatureExperiments.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NatureExperiments.Services.Services
{
    public class NatureExperimentsService : INatureExperimentsService
    {
        private readonly INatureExperimentsRepository _natureExperimentsRepository;

        public NatureExperimentsService(INatureExperimentsRepository natureExperimentsRepository)
        {
            _natureExperimentsRepository = natureExperimentsRepository;
        }


    }
}
