using System.Threading.Tasks;
using Lightest.TestingService.ResponsModels;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;

namespace Lightest.Tests.Api.Tests.TestingController
{
    public class ReportNewServer : BaseTest
    {
        private readonly ServerStatusResponse _server;

        public ReportNewServer()
        {
            _server = new ServerStatusResponse
            {
                ServerVersion = "a"
            };
        }

        [Fact]
        public async Task IpSet()
        {
            _server.Ip = "2.2.2.2";

            var result = await _controller.ReportNewServer(_server);
            Assert.IsAssignableFrom<OkResult>(result);

            _testingServiceMock.Verify(m => m.ReportNewServer(
                It.Is<ServerStatusResponse>(s => s.Ip == _server.Ip && s.ServerVersion == _server.ServerVersion)),
                Times.Once);
        }

        [Fact]
        public async Task IpNotSet()
        {
            var result = await _controller.ReportNewServer(_server);
            Assert.IsAssignableFrom<OkResult>(result);

            _testingServiceMock.Verify(m => m.ReportNewServer(
                It.Is<ServerStatusResponse>(s => s.Ip == DefaultIp && s.ServerVersion == _server.ServerVersion)),
                Times.Once);
        }
    }
}
