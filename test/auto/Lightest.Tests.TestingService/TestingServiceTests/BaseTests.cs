using System.Net;
using Lightest.Data;
using Lightest.TestingService.Interfaces;
using Lightest.Tests.DefaultMocks;
using Moq;
using NUnit.Framework;

namespace Lightest.Tests.TestingService.TestingServiceTests
{
    [TestFixture]
    public abstract class BaseTests
    {
        protected readonly RelationalDbContext _context = MockDatabase.Context;

        protected ITestingService _testingService =>
            new Lightest.TestingService.DefaultServices.TestingService(_serverRepoMock.Object, _context, _factoryMock.Object);

        protected Mock<IServerRepository> _serverRepoMock;
        protected Mock<ITransferServiceFactory> _factoryMock;
        protected Mock<ITransferService> _transferMock;

        [SetUp]
        public void BaseSetUp()
        {
            _serverRepoMock = new Mock<IServerRepository>();
            _transferMock = new Mock<ITransferService>();
            _factoryMock = new Mock<ITransferServiceFactory>();
            _factoryMock.Setup(f => f.Create(It.IsAny<IPAddress>(), It.IsAny<int>()))
                .Returns(_transferMock.Object);
        }
    }
}
