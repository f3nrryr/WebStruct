using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CompModels.ModelsAlghoritms.Interfaces
{
    public interface IAlgorithm
    {
        public Task CalculateAsync(long calculationRequestId);
    }
}
