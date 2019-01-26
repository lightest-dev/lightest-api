using System;
using System.Threading.Tasks;
using Lightest.Data.Models.TaskModels;
using Lightest.TestingService.Models;

namespace Lightest.TestingService.Interfaces
{
    public interface ITestingService
    {
        Task<bool> BeginTesting(IUpload upload);

        Task ReportResult(CheckerResult result);

        Task ReportFreeServer(NewServer server);

        void RemoveCachedChecker(Guid checkerId);
    }
}
