using System.Net;
using Lightest.Api.Models;

namespace Lightest.Api.Services
{
    public interface IServerRepository
    {
        void ReportFreeServer(IPAddress ip);

        TestingServer GetFreeServer();

        void RemoveCachedCheckers(int checkerId);
    }
}
