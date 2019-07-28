using System.Threading.Tasks;
using Lightest.Data.Models;
using Lightest.TestingService.Models;
using Moq;
using Xunit;

namespace Lightest.Tests.TestingService.TestingServiceTests
{
    public class ReportingTests : BaseTests
    {
        private readonly NewServer _testServer;

        public ReportingTests()
        {
            _testServer = new NewServer
            {
                Ip = "1",
                ServerVersion = "12"
            };
        }

        [Fact]
        public async Task ReportBrokenServerTest()
        {
            await _testingService.ReportBrokenServer(_testServer);

            _serverRepoMock.Verify(s => s.AddBrokenServer(It.Is<TestingServer>(ts => ts.Ip == _testServer.Ip)), Times.Once);
            _serverRepoMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ReportNewServerTest()
        {
            await _testingService.ReportNewServer(_testServer);

            _serverRepoMock.Verify(s => s.AddNewServer(It.Is<TestingServer>(ts => ts.Ip == _testServer.Ip)), Times.Once);
            _serverRepoMock.VerifyNoOtherCalls();
        }
    }
}
