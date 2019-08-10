using System;
using System.Linq;
using Lightest.Data.Models;
using Lightest.TestingService.DefaultServices;
using Lightest.TestingService.Interfaces;
using Xunit;

namespace Lightest.Tests.TestingService.ServerRepositoryTests
{
    public class AddCachedChecker : BaseTest
    {
        private readonly IServerRepository _repo;
        private readonly TestingServer _testServer;
        private readonly Checker _testChecker;

        public AddCachedChecker()
        {
            _repo = new ServerRepository(_context);
            _testServer = new TestingServer
            {
                Ip = "1",
                Status = ServerStatus.Free,
                Port = 2,
                Version = "1"
            };
            _testChecker = new Checker
            {
                Id = Guid.NewGuid(),
                Name = "test",
                Code = "code"
            };
        }

        [Fact]
        public void Existing()
        {
            _context.Servers.Add(_testServer);
            _context.SaveChanges();

            _repo.AddCachedChecker(_testServer, _testChecker);
            Assert.Equal(1, _context.CachedCheckers.Count());

            var checker = _context.CachedCheckers.First();
            Assert.Equal(_testChecker.Id, checker.CheckerId);
        }

        [Fact]
        public void NonExistent()
        {
            Assert.Throws<ArgumentException>(() =>
            {
                _repo.AddCachedChecker(_testServer, _testChecker);
            });
        }
    }
}
