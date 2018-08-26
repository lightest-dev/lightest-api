namespace Lightest.Api.Models
{
    public class TestingRequest
    {
        public int UploadId { get; set; }

        public int MemoryLimit { get; set; }

        public int TimeLimit { get; set; }

        public string Extension { get; set; }

        public string Type { get; set; }

        public int TestsCount { get; set; }
    }
}
