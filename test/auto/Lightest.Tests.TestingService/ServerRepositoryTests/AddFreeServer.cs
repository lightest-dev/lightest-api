using System.Linq;
using Lightest.Data;
using Lightest.Data.Models;
using Lightest.TestingService.DefaultServices;
using Lightest.TestingService.Interfaces;
using Lightest.Tests.DefaultMocks;
using NUnit.Framework;

namespace Lightest.Tests.TestingService.ServerRepositoryTests
{
    [TestFixture]
    public class AddFreeServer
    {
        private readonly RelationalDbContext _context = MockDatabase.Context;
        private IServerRepository _repo;
        private TestingServer _testServer;

        [SetUp]
        public void CleanServers()
        {
            _context.Servers.RemoveRange(_context.Servers);
            _context.SaveChanges();
            _repo = new ServerRepository(_context);
            _testServer = new TestingServer
            {
                Ip = "1",
                Status = ServerStatus.Busy
            };
        }

        [Test]
        public void TestEmpty()
        {
            _repo.AddFreeServer(_testServer);
            Assert.Zero(_context.Servers.Count());
        }

        [Test]
        public void TestExisting()
        {
            _context.Servers.Add(_testServer);
            _context.SaveChanges();
            var copy = new TestingServer
            {
                Ip = "1"
            };
            _repo.AddFreeServer(copy);

            var entry = _context.Servers.First();

            Assert.AreEqual(1, _context.Servers.Count());
            Assert.AreEqual("1", entry.Ip);
            Assert.AreEqual(ServerStatus.Free, entry.Status);
        }
    }
}
