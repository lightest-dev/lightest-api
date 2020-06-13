using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Lightest.Data.Models;
using Xunit;

namespace Lightest.Tests.TestingService.DefaultTestingServiceTests
{
    public class ReportNewServer : BaseTests
    {
        private const string Ip = "2.2.2.2";

        [Fact]
        public void New()
        {
            _testingService.ReportNewServer(Ip);
            Assert.Equal(1, _context.Servers.Count());

            var entry = _context.Servers.First();

            Assert.Equal(ServerStatus.Free, entry.Status);
            Assert.Equal(Ip, entry.Ip);
            Assert.Equal(443, entry.Port);
        }

        [Fact]
        public void Existing()
        {
            _context.Servers.Add(new TestingServerInfo
            {
                Ip = Ip,
                Port = 1,
                Status = ServerStatus.Busy
            });
            _context.SaveChanges();

            _testingService.ReportNewServer(Ip);
            Assert.Equal(1, _context.Servers.Count());

            var entry = _context.Servers.First();

            Assert.Equal(ServerStatus.Free, entry.Status);
            Assert.Equal(Ip, entry.Ip);
            Assert.Equal(443, entry.Port);
        }
    }
}
