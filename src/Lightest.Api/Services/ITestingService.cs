using Lightest.Data.Models;
using Lightest.Data.Models.TaskModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Lightest.Api.Services
{
    public interface ITestingService
    {
        Task<bool> BeginTesting(CodeUpload task);

        Task<bool> CheckStatus(CodeUpload task);

        Task<double> GetResult(CodeUpload task);

        Task<bool> BeginTesting(ArchiveUpload task);

        Task<bool> CheckStatus(ArchiveUpload task);

        Task<double> GetResult(ArchiveUpload task);
    }
}
