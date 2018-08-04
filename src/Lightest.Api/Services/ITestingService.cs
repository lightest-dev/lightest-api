using Lightest.Data.Models.TaskModels;
using System.Threading.Tasks;

namespace Lightest.Api.Services
{
    public interface ITestingService
    {
        Task<bool> BeginTesting(IUpload upload);

        Task ReportResult(CheckerResult result);
    }
}