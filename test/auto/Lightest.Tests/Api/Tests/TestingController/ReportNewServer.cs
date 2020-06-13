using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;

namespace Lightest.Tests.Api.Tests.TestingController
{
    public class ReportNewServer : BaseTest
    {
        [Fact]
        public async Task ServerAdded()
        {
            var result = await _controller.ReportNewServer();
            Assert.IsAssignableFrom<OkResult>(result);

            _testingServiceMock.Verify(m => m.ReportNewServer(
                It.Is<string>(s => s == DefaultIp)),
                Times.Once);
        }
    }
}
