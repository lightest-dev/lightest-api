using System.Net;
using Lightest.Data.Mongo.Models.Services;
using Lightest.TestingService.Interfaces;
using Moq;

namespace Lightest.Tests.TestingService.TestingServiceTests
{
    public abstract class BaseTests : BaseTest
    {
        protected ITestingService _testingService =>
            new Lightest.TestingService.DefaultServices.DefaultTestingService(_serverRepoMock.Object, _context, _factoryMock.Object, _uploadDataRepository.Object);

        protected readonly Mock<IServerRepository> _serverRepoMock;
        protected readonly Mock<ITransferServiceFactory> _factoryMock;
        protected readonly Mock<ITransferService> _transferMock;
        protected readonly Mock<IUploadDataRepository> _uploadDataRepository;

        public BaseTests()
        {
            _serverRepoMock = new Mock<IServerRepository>();
            _transferMock = new Mock<ITransferService>();
            _factoryMock = new Mock<ITransferServiceFactory>();
            _factoryMock.Setup(f => f.Create(It.IsAny<IPAddress>(), It.IsAny<int>()))
                .Returns(_transferMock.Object);
            _uploadDataRepository = new Mock<IUploadDataRepository>();
        }
    }
}
