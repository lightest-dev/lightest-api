using System.Threading.Tasks;
using Lightest.TestingService.Requests;
using Moq;
using NUnit.Framework;

namespace Lightest.Tests.TestingService.TestingServiceTests
{
    [TestFixture]
    public class BeginTestingTests : BaseTests
    {
        [SetUp]
        public void SetUp()
        {
            _transferMock.Setup(t => t.SendMessage(It.IsNotNull<string>()))
                .Returns(Task.FromResult(true));

            _transferMock.Setup(t => t.SendFile(It.IsNotNull<FileRequest>(), It.IsNotNull<byte[]>()))
                .Returns(Task.FromResult(true));
        }

        [Test]
        public async Task CachedCheckerTest()
        {
        }

        [Test]
        public async Task NonCachedCheckerTest()
        {
        }
    }
}
