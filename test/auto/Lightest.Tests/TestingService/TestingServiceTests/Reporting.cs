using System.Threading.Tasks;
using Lightest.Data.Models;
using Lightest.TestingService.ResponsModels;
using Moq;
using Xunit;

namespace Lightest.Tests.TestingService.TestingServiceTests
{
    public class Reporting : BaseTests
    {
        private readonly ServerStatusResponse _testServer;

        public Reporting() => _testServer = new ServerStatusResponse
        {
            Ip = "1",
            ServerVersion = "12"
        };

        [Fact]
        public async Task ReportBrokenServer()
        {
            await _testingService.ReportBrokenServer(_testServer);

            _serverRepoMock.Verify(s => s.AddBrokenServer(It.Is<TestingServer>(ts => ts.Ip == _testServer.Ip)), Times.Once);
            _serverRepoMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ReportNewServer()
        {
            await _testingService.ReportNewServer(_testServer);

            _serverRepoMock.Verify(s => s.AddNewServer(It.Is<TestingServer>(ts => ts.Ip == _testServer.Ip)), Times.Once);
            _serverRepoMock.VerifyNoOtherCalls();
        }
    }
}
