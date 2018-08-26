using System.Collections.Generic;
using System.Net;

namespace Lightest.Api.Models
{
    public class TestingServer
    {
        public TestingServer()
        {
            CachedCheckerIds = new List<int>();
        }

        public IPAddress ServerAddress { get; set; }

        public List<int> CachedCheckerIds { get; }

        public ServerStatus Status { get; set; }
    }
}
