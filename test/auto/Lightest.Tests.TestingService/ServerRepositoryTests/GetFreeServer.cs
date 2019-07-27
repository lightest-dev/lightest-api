using Lightest.Data;
using Lightest.TestingService.DefaultServices;
using Lightest.TestingService.Interfaces;
using Lightest.Tests.DefaultMocks;
using NUnit.Framework;

namespace Lightest.Tests.TestingService.ServerRepositoryTests
{
    [TestFixture]
    public class GetFreeServer
    {
        private readonly RelationalDbContext _context = MockDatabase.Context;
        private IServerRepository _repo;

        [SetUp]
        public void CleanServers()
        {
            _context.Servers.RemoveRange(_context.Servers);
            _context.SaveChanges();
            _repo = new ServerRepository(_context);
        }

        [Test]
        public void TestEmpty()
        {
            var server = _repo.GetFreeServer();
            Assert.IsNull(server);
        }

        [Test]
        public void TestNoFree()
        {
            _context.Servers.Add(new Data.Models.TestingServer
            {
                Status = Data.Models.ServerStatus.Busy
            });
            _context.Servers.Add(new Data.Models.TestingServer
            {
                Status = Data.Models.ServerStatus.NotResponding
            });
            _context.SaveChanges();

            var server = _repo.GetFreeServer();
            Assert.IsNull(server);
        }

        [Test]
        public void TestFree()
        {
            _context.Servers.Add(new Data.Models.TestingServer
            {
                Status = Data.Models.ServerStatus.Busy
            });
            _context.Servers.Add(new Data.Models.TestingServer
            {
                Status = Data.Models.ServerStatus.NotResponding
            });
            _context.Servers.Add(new Data.Models.TestingServer
            {
                Status = Data.Models.ServerStatus.Free
            });
            _context.SaveChanges();

            var server = _repo.GetFreeServer();
            Assert.IsNotNull(server);
            Assert.AreEqual(server.Status, Data.Models.ServerStatus.Free);
        }
    }
}
