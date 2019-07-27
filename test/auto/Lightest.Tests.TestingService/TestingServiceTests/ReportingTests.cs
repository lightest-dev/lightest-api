using System.Threading.Tasks;
using Lightest.Data.Models;
using Lightest.TestingService.Models;
using Moq;
using NUnit.Framework;

namespace Lightest.Tests.TestingService.TestingServiceTests
{
    public class ReportingTests : BaseTests
    {
        private NewServer _testServer;

        [SetUp]
        public void SetUp()
        {
            _context.Servers.RemoveRange(_context.Servers);
            _context.SaveChanges();

            _testServer = new NewServer
            {
                Ip = "1",
                ServerVersion = "12"
            };
        }

        [Test]
        public async Task ReportBrokenServerTest()
        {
            await _testingService.ReportBrokenServer(_testServer);

            _serverRepoMock.Verify(s => s.AddBrokenServer(It.IsNotNull<TestingServer>()), Times.Once);
            _serverRepoMock.VerifyNoOtherCalls();
        }

        [Test]
        public async Task ReportNewServerTest()
        {
            await _testingService.ReportNewServer(_testServer);

            _serverRepoMock.Verify(s => s.AddNewServer(It.IsNotNull<TestingServer>()), Times.Once);
            _serverRepoMock.VerifyNoOtherCalls();
        }
    }
}
