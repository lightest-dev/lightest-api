using System.Threading.Tasks;

namespace Lightest.TestingService.Interfaces
{
    public interface ITestingRunner
    {
        Task TryStartNewWorker();

        Task UpdateServerStatuses();
    }
}
