using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CompModels.ModelsAlghoritms.Handler
{
    public interface ICalculateRequestsHandler
    {
        Task HandleAsync();
    }
}
