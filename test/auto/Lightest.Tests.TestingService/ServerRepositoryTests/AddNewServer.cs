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
    public class AddNewServer
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
                Status = ServerStatus.Busy,
                Port = 2,
                Version = "1"
            };
        }

        [Test]
        public void TestNew()
        {
            _repo.AddNewServer(_testServer);
            Assert.AreEqual(1, _context.Servers.Count());

            var entry = _context.Servers.First();

            Assert.AreEqual(entry.Status, ServerStatus.Busy);
        }

        [Test]
        public void TestExisting()
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

            Assert.AreEqual(1, _context.Servers.Count());
            Assert.AreEqual("1", entry.Ip);
            Assert.AreEqual(ServerStatus.Free, entry.Status);
            Assert.AreEqual(3, entry.Port);
            Assert.AreEqual("newVersion", entry.Version);
        }
    }
}
