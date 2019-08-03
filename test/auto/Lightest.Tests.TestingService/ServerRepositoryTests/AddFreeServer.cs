using System.Linq;
using Lightest.Data.Models;
using Lightest.TestingService.DefaultServices;
using Lightest.TestingService.Interfaces;
using Xunit;

namespace Lightest.Tests.TestingService.ServerRepositoryTests
{
    public class AddFreeServer : BaseTest
    {
        private readonly IServerRepository _repo;
        private readonly TestingServer _testServer;

        public AddFreeServer()
        {
            _repo = new ServerRepository(_context);
            _testServer = new TestingServer
            {
                Ip = "1",
                Status = ServerStatus.Busy
            };
        }

        [Fact]
        public void Empty()
        {
            _repo.AddFreeServer(_testServer);
            Assert.Equal(0, _context.Servers.Count());
        }

        [Fact]
        public void Existing()
        {
            _context.Servers.Add(_testServer);
            _context.SaveChanges();
            var copy = new TestingServer
            {
                Ip = "1"
            };
            _repo.AddFreeServer(copy);

            var entry = _context.Servers.First();

            Assert.Equal(1, _context.Servers.Count());
            Assert.Equal("1", entry.Ip);
            Assert.Equal(ServerStatus.Free, entry.Status);
        }
    }
}
