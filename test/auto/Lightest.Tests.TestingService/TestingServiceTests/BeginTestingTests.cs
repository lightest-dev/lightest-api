using System.Threading.Tasks;
using Lightest.TestingService.Requests;
using Moq;
using Xunit;

namespace Lightest.Tests.TestingService.TestingServiceTests
{
    public class BeginTestingTests : BaseTests
    {
        public BeginTestingTests()
        {
            _transferMock.Setup(t => t.SendMessage(It.IsNotNull<string>()))
                .Returns(Task.FromResult(true));

            _transferMock.Setup(t => t.SendFile(It.IsNotNull<FileRequest>(), It.IsNotNull<byte[]>()))
                .Returns(Task.FromResult(true));
        }

        [Fact]
        public async Task CachedCheckerTest()
        {
        }

        [Fact]
        public async Task NonCachedCheckerTest()
        {
        }
    }
}
