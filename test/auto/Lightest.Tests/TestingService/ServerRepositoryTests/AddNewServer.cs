using System.Linq;
using Lightest.Data.Models;
using Lightest.TestingService.DefaultServices;
using Lightest.TestingService.Interfaces;
using Xunit;

namespace Lightest.Tests.TestingService.ServerRepositoryTests
{
    public class AddNewServer : BaseTest
    {
        private readonly IServerRepository _repo;
        private readonly TestingServer _testServer;

        public AddNewServer()
        {
            _repo = new ServerRepository(_context);
            _testServer = new TestingServer
            {
                Ip = "1",
                Status = ServerStatus.Busy,
                Port = 2,
                Version = "1"
            };
        }

        [Fact]
        public void New()
        {
            _repo.AddNewServer(_testServer);
            Assert.Equal(1, _context.Servers.Count());

            var entry = _context.Servers.First();

            Assert.Equal(ServerStatus.Busy, entry.Status);
        }

        [Fact]
        public void Existing()
        {
            _context.Servers.Add(_testServer);
            _context.SaveChanges();
            var copy = new TestingServer
            {
                Ip = "1",
                Status = ServerStatus.Free,
                Port = 3,
                Version = "newVersion"
            };
            _repo.AddNewServer(copy);

            var entry = _context.Servers.First();

            Assert.Equal(1, _context.Servers.Count());
            Assert.Equal("1", entry.Ip);
            Assert.Equal(ServerStatus.Free, entry.Status);
            Assert.Equal(3, entry.Port);
            Assert.Equal("newVersion", entry.Version);
        }
    }
}
