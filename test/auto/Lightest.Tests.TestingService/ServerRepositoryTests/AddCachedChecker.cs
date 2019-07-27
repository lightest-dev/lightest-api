using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Lightest.Data;
using Lightest.Data.Models;
using Lightest.TestingService.DefaultServices;
using Lightest.TestingService.Interfaces;
using Lightest.Tests.DefaultMocks;
using NUnit.Framework;

namespace Lightest.Tests.TestingService.ServerRepositoryTests
{
    [TestFixture]
    public class AddCachedChecker
    {
        private readonly RelationalDbContext _context = MockDatabase.Context;
        private IServerRepository _repo;
        private TestingServer _testServer;
        private Checker _testChecker;


        [SetUp]
        public void CleanServers()
        {
            _context.Servers.RemoveRange(_context.Servers);
            _context.SaveChanges();
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

        [Test]
        public void TestExisting()
        {
            _context.Servers.Add(_testServer);
            _context.SaveChanges();

            _repo.AddCachedChecker(_testServer, _testChecker);
            Assert.AreEqual(1, _context.CachedCheckers.Count());

            var checker = _context.CachedCheckers.First();
            Assert.AreEqual(_testChecker.Id, checker.CheckerId);
        }

        [Test]
        public void TestNonExisting()
        {
            Assert.Throws<ArgumentException>(() =>
            {
                _repo.AddCachedChecker(_testServer, _testChecker);
            });
        }
    }
}
