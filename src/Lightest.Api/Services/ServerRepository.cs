using System.Collections.Generic;
using System.Net;
using Lightest.Api.Models;

namespace Lightest.Api.Services
{
    public class ServerRepository : IServerRepository
    {
        private readonly List<TestingServer> _availableServers;

        public ServerRepository()
        {
            _availableServers = new List<TestingServer>();
        }

        public TestingServer GetFreeServer()
        {
            var server = _availableServers.Find(s => s.Status == ServerStatus.Free);
            server.Status = ServerStatus.Busy;
            return server;
        }

        public void RemoveCachedCheckers(int checkerId)
        {
            foreach (var server in _availableServers)
            {
                server.CachedCheckerIds.Remove(checkerId);
            }
        }

        public void ReportFreeServer(IPAddress ip)
        {
            var server = _availableServers.Find(s => s.ServerAdress == ip);
            if (server == null)
            {
                server = new TestingServer
                {
                    ServerAdress = ip
                };
                _availableServers.Add(server);
            }
            server.Status = ServerStatus.Free;
        }
    }
}
