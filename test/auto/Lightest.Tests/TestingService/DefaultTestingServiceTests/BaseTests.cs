using Lightest.TestingService.Interfaces;
using Moq;

namespace Lightest.Tests.TestingService.DefaultTestingServiceTests
{
    public abstract class BaseTests : BaseTest
    {
        protected ITestingService _testingService =>
            new Lightest.TestingService.DefaultServices.DefaultTestingService(_context, _testingRunner.Object);

        protected readonly Mock<ITestingRunner> _testingRunner;

        public BaseTests()
        {
            _testingRunner = new Mock<ITestingRunner>();
        }
    }
}
