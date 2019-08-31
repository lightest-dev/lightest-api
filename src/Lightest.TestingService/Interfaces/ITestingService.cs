using System.Threading.Tasks;
using Lightest.Data.Models.TaskModels;
using Lightest.TestingService.Models;

namespace Lightest.TestingService.Interfaces
{
    public interface ITestingService
    {
        Task<bool> BeginTesting(Upload upload);

        Task ReportResult(CheckerResult result);

        Task ReportNewServer(NewServer server);

        Task ReportBrokenServer(NewServer server);

        Task StartNextTesting();
    }
}
