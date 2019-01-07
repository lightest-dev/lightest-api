using System.Net;
using Lightest.TestingService.Models;

namespace Lightest.TestingService.Interfaces
{
    public interface IServerRepository
    {
        void ReportFreeServer(IPAddress ip);

        TestingServer GetFreeServer();

        void RemoveCachedCheckers(int checkerId);

        int ServersCount { get; }
    }
}
