using Lightest.Data.Models.TaskModels;
using System.Threading.Tasks;

namespace Lightest.Api.Services
{
    public interface ITestingService
    {
        Task<bool> BeginTesting(CodeUpload task);

        Task<bool> BeginTesting(ArchiveUpload task);

        Task<bool> CheckStatus(IUpload task);

        Task<double> GetResult(IUpload task);

        Task ReportResult(CheckerResult result);
    }
}