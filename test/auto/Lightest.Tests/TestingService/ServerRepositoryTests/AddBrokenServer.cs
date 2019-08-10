using System.Linq;
using Lightest.Data.Models;
using Lightest.TestingService.DefaultServices;
using Lightest.TestingService.Interfaces;
using Xunit;

namespace Lightest.Tests.TestingService.ServerRepositoryTests
{
    public class AddBrokenServer : BaseTest
    {
        private readonly IServerRepository _repo;
        private readonly TestingServer _testServer;

        public AddBrokenServer()
        {
            _repo = new ServerRepository(_context);
            _testServer = new TestingServer
            {
                Ip = "1",
                Status = ServerStatus.Free,
                Port = 2,
                Version = "1"
            };
        }

        [Fact]
        public void New()
        {
            _repo.AddBrokenServer(_testServer);
            Assert.Equal(0, _context.Servers.Count());
        }

        [Fact]
        public void Existing()
        {
            _context.Servers.Add(_testServer);
            _context.SaveChanges();
            var copy = new TestingServer
            {
                Ip = "1",
                Status = ServerStatus.Busy,
                Port = 3,
                Version = "newVersion"
            };
            _repo.AddBrokenServer(copy);

            var entry = _context.Servers.First();

            Assert.Equal(1, _context.Servers.Count());
            Assert.Equal(ServerStatus.NotResponding, entry.Status);
        }
    }
}
