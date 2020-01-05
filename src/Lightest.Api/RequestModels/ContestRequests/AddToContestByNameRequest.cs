using System;

namespace Lightest.Api.RequestModels.ContestRequests
{
    public class AddToContestByNameRequest
    {
        public string Pattern { get; set; }

        public Guid ContestId { get; set; }
    }
}
