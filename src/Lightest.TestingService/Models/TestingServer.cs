using System;
using System.Collections.Generic;
using System.Net;

namespace Lightest.TestingService.Models
{
    public class TestingServer
    {
        public TestingServer()
        {
            CachedCheckerIds = new List<Guid>();
        }

        public IPAddress ServerAddress { get; set; }

        public List<Guid> CachedCheckerIds { get; }

        public ServerStatus Status { get; set; }
    }
}
