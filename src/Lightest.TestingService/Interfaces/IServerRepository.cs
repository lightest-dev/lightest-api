using System;
using Lightest.Data.Models;

namespace Lightest.TestingService.Interfaces
{
    public interface IServerRepository
    {
        void AddFreeServer(TestingServer server);

        void AddBrokenServer(TestingServer server);

        TestingServer GetFreeServer();

        void RemoveCachedCheckers(Guid checkerId);

        void AddCachedChecker(TestingServer server, Guid checkerId);

        void AddNewServer(TestingServer server);

        int ServersCount { get; }
    }
}
