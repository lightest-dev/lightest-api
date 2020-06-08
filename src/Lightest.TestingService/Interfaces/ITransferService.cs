using System;
using System.Threading.Tasks;
using GrpcServices;

namespace Lightest.TestingService.Interfaces
{
    /// <summary>
    /// Use to transfer data to testing container. Listener in container should understand the
    /// format of data.
    /// </summary>
    public interface ITransferService : IDisposable
    {
        Task<CheckerResponse> SendChecker(CheckerRequest request);
        Task<TestingResponse> SendUpload(TestingRequest request);
        Task<CheckerStatusResponse> GetStatus();
        Task<CheckerStatusResponse> GetStatus(CheckerStatusRequest request);
    }
}
