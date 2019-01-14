using System;
using System.Net;
using Lightest.TestingService.Models;

namespace Lightest.TestingService.Interfaces
{
    public interface IServerRepository
    {
        void AddFreeServer(IPAddress ip);

        void AddBrokenServer(IPAddress ip);

        TestingServer GetFreeServer();

        void RemoveCachedCheckers(Guid checkerId);

        int ServersCount { get; }
    }
}
