using Lightest.Data.Models.TaskModels;
using System.Threading.Tasks;

namespace Lightest.Api.Services
{
    public interface ITestingService
    {
        Task<bool> BeginTesting(IUpload task);

        Task<bool> CheckStatus(IUpload task);

        Task<double> GetResult(IUpload task);

        Task ReportResult(CheckerResult result);
    }
}