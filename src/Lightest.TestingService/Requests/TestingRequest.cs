using System;
using System.Text.Json;

namespace Lightest.TestingService.Requests
{
    public class TestingRequest : BaseRequest
    {
        public Guid UploadId { get; set; }

        public int MemoryLimit { get; set; }

        public int TimeLimit { get; set; }

        public Guid CheckerId { get; set; }

        public int TestsCount { get; set; }

        public override string Type => "upload";

        public override string ToString() => JsonSerializer.Serialize(this);
    }
}
