using System;
using System.Linq;
using Lightest.Data;
using Lightest.Data.Models;
using Lightest.TestingService.Interfaces;

namespace Lightest.TestingService.DefaultServices
{
    public class ServerRepository : IServerRepository
    {
        private readonly RelationalDbContext _context;

        public ServerRepository(RelationalDbContext context) => _context = context;

        public int ServersCount => _context.Servers.Count();

        public TestingServer GetFreeServer()
        {
            var server = _context.Servers.FirstOrDefault(s => s.Status == ServerStatus.Free);
            return server;
        }

        public void AddFreeServer(TestingServer server)
        {
            var entry = _context.Servers.Find(server.Ip);
            if (entry != null)
            {
                entry.Status = ServerStatus.Free;
                _context.SaveChanges();
            }
        }

        public void AddNewServer(TestingServer server)
        {
            var entry = _context.Servers.Find(server.Ip);
            if (entry == null)
            {
                _context.Servers.Add(server);
            }
            else
            {
                entry.Port = server.Port;
                entry.Version = server.Version;
                entry.Status = server.Status;
                var checkersToDelete = _context.CachedCheckers
                    .Where(c => c.ServerIp == server.Ip);
                _context.CachedCheckers.RemoveRange(checkersToDelete);
            }
            _context.SaveChanges();
        }

        public void AddBrokenServer(TestingServer server)
        {
            var entry = _context.Servers.Find(server.Ip);
            if (entry != null)
            {
                entry.Status = ServerStatus.NotResponding;
                _context.SaveChanges();
            }
        }

        public void AddCachedChecker(TestingServer server, Checker checker)
        {
            var serverExists = _context.Servers.Any(s => s.Ip == server.Ip);
            if (!serverExists)
            {
                throw new ArgumentException("Server does not exist", nameof(server));
            }

            var cachedChecker = new ServerChecker
            {
                Server = server,
                Checker = checker
            };
            _context.CachedCheckers.Add(cachedChecker);
            _context.SaveChanges();
        }
    }
}
