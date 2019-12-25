using Lightest.TestingService.DefaultServices;
using Lightest.TestingService.Interfaces;
using Xunit;

namespace Lightest.Tests.TestingService.ServerRepositoryTests
{
    public class GetFreeServer : BaseTest
    {
        private readonly IServerRepository _repo;

        public GetFreeServer()
        {
            _repo = new ServerRepository(_context);
        }

        [Fact]
        public void Empty()
        {
            var server = _repo.GetFreeServer();
            Assert.Null(server);
        }

        [Fact]
        public void NoFree()
        {
            _context.Servers.Add(new Data.Models.TestingServer
            {
                Status = Data.Models.ServerStatus.Busy,
                Ip = "1"
            });
            _context.Servers.Add(new Data.Models.TestingServer
            {
                Status = Data.Models.ServerStatus.NotResponding,
                Ip = "2"
            });
            _context.SaveChanges();

            var server = _repo.GetFreeServer();
            Assert.Null(server);
        }

        [Fact]
        public void Free()
        {
            _context.Servers.Add(new Data.Models.TestingServer
            {
                Status = Data.Models.ServerStatus.Busy,
                Ip = "1"
            });
            _context.Servers.Add(new Data.Models.TestingServer
            {
                Status = Data.Models.ServerStatus.NotResponding,
                Ip = "2"
            });
            _context.Servers.Add(new Data.Models.TestingServer
            {
                Status = Data.Models.ServerStatus.Free,
                Ip = "3"
            });
            _context.SaveChanges();

            var server = _repo.GetFreeServer();
            Assert.NotNull(server);
            Assert.Equal(Data.Models.ServerStatus.Free, server.Status);
        }
    }
}
