using System.Threading.Tasks;
using Lightest.Api.RequestModels.TestingRequests;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;

namespace Lightest.Tests.Api.Tests.TestingController
{
    public class ReportNewServer : BaseTest
    {
        [Fact]
        public async Task RequestIsEmpty()
        {
            var result = await _controller.ReportNewServer(null);
            Assert.IsAssignableFrom<OkResult>(result);

            _testingServiceMock.Verify(m => m.ReportNewServer(
                It.Is<string>(s => s == DefaultIp)),
                Times.Once);
        }

        [Theory]
        [InlineData(default(string))]
        [InlineData("")]
        [InlineData("  \t\n")]
        public async Task RequestIpIsNotSet(string ip)
        {
            var result = await _controller.ReportNewServer(new NewServerRequest
            {
                Ip = ip
            });
            Assert.IsAssignableFrom<OkResult>(result);

            _testingServiceMock.Verify(m => m.ReportNewServer(
                It.Is<string>(s => s == DefaultIp)),
                Times.Once);
        }

        [Fact]
        public async Task RequestIpIsSet()
        {
            const string ip = "4.4.4.4";
            var result = await _controller.ReportNewServer(new NewServerRequest
            {
                Ip = ip
            });
            Assert.IsAssignableFrom<OkResult>(result);

            _testingServiceMock.Verify(m => m.ReportNewServer(
                It.Is<string>(s => s == ip)),
                Times.Once);
        }
    }
}
