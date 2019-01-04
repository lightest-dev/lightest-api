using System.Threading.Tasks;
using Lightest.Data.Models.TaskModels;

namespace Lightest.TestingService.Interfaces
{
    public interface ITestingService
    {
        Task<bool> BeginTesting(IUpload upload);

        Task ReportResult(CheckerResult result);
    }
}
