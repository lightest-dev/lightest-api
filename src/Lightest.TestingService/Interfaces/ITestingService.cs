using System.Threading.Tasks;
using Lightest.Data.Models.TaskModels;
using Lightest.TestingService.ResponsModels;

namespace Lightest.TestingService.Interfaces
{
    public interface ITestingService
    {
        Task<bool> BeginTesting(Upload upload);

        Task ReportResult(CheckingResponse result);

        Task ReportNewServer(ServerStatusResponse server);

        Task ReportBrokenServer(ServerStatusResponse server);

        Task StartNextTesting();
    }
}
