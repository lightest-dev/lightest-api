using System;
using System.Collections.Generic;
using System.Net;

namespace Lightest.TestingService.Models
{
    public class TestingServer
    {
        public TestingServer(IPAddress address)
        {
            CachedCheckerIds = new List<Guid>();
            ServerAddress = address;
        }

        public IPAddress ServerAddress { get; }

        public List<Guid> CachedCheckerIds { get; }

        public ServerStatus Status { get; set; }
    }
}
