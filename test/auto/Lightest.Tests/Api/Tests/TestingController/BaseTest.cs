using Lightest.TestingService.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Moq;

namespace Lightest.Tests.Api.Tests.TestingController
{
    public abstract class BaseTest : Api.BaseTest
    {
        protected Lightest.Api.Controllers.TestingController _controller
        {
            get
            {
                var controller = new Lightest.Api.Controllers.TestingController(_testingServiceMock.Object,
                    _context, _accessorMock.Object, _loggerMock.Object);
                return controller;
            }
        }

        protected readonly Mock<ITestingService> _testingServiceMock;
        protected readonly Mock<IHttpContextAccessor> _accessorMock;
        protected readonly Mock<ILogger<Lightest.Api.Controllers.TestingController>> _loggerMock;
        protected readonly string DefaultIp = "1.1.1.1";

        public BaseTest()
        {
            _testingServiceMock = new Mock<ITestingService>();

            _accessorMock = new Mock<IHttpContextAccessor>();
            _accessorMock.Setup(o => o.HttpContext.Connection.RemoteIpAddress)
                .Returns(System.Net.IPAddress.Parse(DefaultIp));

            _loggerMock = new Mock<ILogger<Lightest.Api.Controllers.TestingController>>();
        }
    }
}
