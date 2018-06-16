using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Lightest.Api.Services
{
    interface ITestingService
    {
        //todo: create specific type
        Task<bool> BeginTestigs(Task task);

        Task<bool> CheckStatus(object task);

        Task<double> GetResult(object task);
    }
}
