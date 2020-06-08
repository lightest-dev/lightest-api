using System.Threading.Tasks;
using GrpcServices;

namespace Lightest.TestingService.Interfaces
{
    public interface IUploadProcessor
    {
        Task<bool> CacheChecker();

        Task ReportCodeResult(TestingResponse result);

        Task<TestingResponse> SendData();
    }
}
