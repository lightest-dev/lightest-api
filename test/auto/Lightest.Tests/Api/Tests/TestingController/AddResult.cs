using System.Threading.Tasks;
using Lightest.TestingService.ResponsModels;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;

namespace Lightest.Tests.Api.Tests.TestingController
{
    public class AddResult : BaseTest
    {
        private readonly CheckingResponse _result;

        public AddResult()
        {
            _result = new CheckingResponse();
        }

        [Fact]
        public async Task IpSet()
        {
            _result.Ip = "2.2.2.2";
            var result = await _controller.AddResult(_result);

            Assert.IsAssignableFrom<OkResult>(result);
            _testingServiceMock.Verify(m => m.ReportResult(It.Is<CheckingResponse>(r => r.Ip == _result.Ip)), Times.Once);
            _testingServiceMock.Verify(m => m.StartNextTesting(), Times.Once);
        }

        [Fact]
        public async Task IpNotSet()
        {
            var result = await _controller.AddResult(_result);

            Assert.IsAssignableFrom<OkResult>(result);
            _testingServiceMock.Verify(m => m.ReportResult(It.Is<CheckingResponse>(r => r.Ip == DefaultIp)), Times.Once);
            _testingServiceMock.Verify(m => m.StartNextTesting(), Times.Once);
        }
    }
}
