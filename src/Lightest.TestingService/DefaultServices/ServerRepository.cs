using System;
using System.Collections.Generic;
using System.Net;
using Lightest.TestingService.Interfaces;
using Lightest.TestingService.Models;

namespace Lightest.TestingService.DefaultServices
{
    public class ServerRepository : IServerRepository
    {
        private readonly List<TestingServer> _availableServers;

        public ServerRepository()
        {
            _availableServers = new List<TestingServer>();
        }

        public int ServersCount => _availableServers.Count;

        public TestingServer GetFreeServer()
        {
            var server = _availableServers.Find(s => s.Status == ServerStatus.Free);
            if (server != null)
            {
                server.Status = ServerStatus.Busy;
            }
            return server;
        }

        public void RemoveCachedCheckers(Guid checkerId)
        {
            foreach (var server in _availableServers)
            {
                server.CachedCheckerIds.Remove(checkerId);
            }
        }

        public void AddFreeServer(IPAddress ip)
        {
            var server = _availableServers.Find(s => Equals(s.ServerAddress, ip));
            if (server == null)
            {
                server = new TestingServer
                {
                    ServerAddress = ip
                };
                _availableServers.Add(server);
            }
            server.Status = ServerStatus.Free;
        }

        public void AddBrokenServer(IPAddress ip)
        {
            var server = _availableServers.Find(s => Equals(s.ServerAddress, ip));
            if (server != null)
            {
                server.Status = ServerStatus.NotResponding;
            }
        }
    }
}
