using System.Threading.Tasks;
using Xunit;

namespace Lightest.Tests.TestingService.TestingServiceTests
{
    public class StartNextTesting : BaseTests
    {
        [Fact]
        public async Task NoTaskInQueue()
        {
            await _testingService.StartNextTesting();
            _factoryMock.VerifyNoOtherCalls();
            _serverRepoMock.VerifyNoOtherCalls();
            _transferMock.VerifyNoOtherCalls();
        }
    }
}
