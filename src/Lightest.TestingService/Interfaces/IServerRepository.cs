using System;
using Lightest.Data.Models;

namespace Lightest.TestingService.Interfaces
{
    public interface IServerRepository
    {
        void AddFreeServer(TestingServer server);

        void AddBrokenServer(TestingServer server);

        TestingServer GetFreeServer();

        void AddCachedChecker(TestingServer server, Checker checker);

        void AddNewServer(TestingServer server);

        int ServersCount { get; }
    }
}
