using System.Threading.Tasks;
using Lightest.Data.Models.TaskModels;
using Lightest.Data.Mongo.Models;
using Lightest.TestingService.ResponsModels;

namespace Lightest.TestingService.Interfaces
{
    public interface ITestingService
    {
        Task<bool> BeginTesting(Upload upload, UploadData uploadData);

        Task ReportResult(CheckingResponse result);

        Task ReportNewServer(ServerStatusResponse server);

        Task ReportBrokenServer(ServerStatusResponse server);

        Task StartNextTesting();
    }
}
