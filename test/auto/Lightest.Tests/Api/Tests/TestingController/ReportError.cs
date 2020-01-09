using System.Threading.Tasks;
using Lightest.TestingService.ResponsModels;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;

namespace Lightest.Tests.Api.Tests.TestingController
{
    public class ReportError : BaseTest
    {
        private readonly TestingErrorResponse _error;

        public ReportError()
        {
            _error = new TestingErrorResponse
            {
                ErrorMessage = "error",
                ServerVersion = "version"
            };
        }

        [Fact]
        public async Task IpSet()
        {
            _error.Ip = "2.2.2.2";

            var result = await _controller.ReportError(_error);
            Assert.IsAssignableFrom<OkResult>(result);

            _testingServiceMock.Verify(m => m.ReportBrokenServer(
                It.Is<ServerStatusResponse>(s => s.Ip == _error.Ip && s.ServerVersion == _error.ServerVersion)),
                Times.Once);
        }

        [Fact]
        public async Task IpNotSet()
        {
            var result = await _controller.ReportError(_error);
            Assert.IsAssignableFrom<OkResult>(result);

            _testingServiceMock.Verify(m => m.ReportBrokenServer(
                It.Is<ServerStatusResponse>(s => s.Ip == DefaultIp && s.ServerVersion == _error.ServerVersion)),
                Times.Once);
        }
    }
}
