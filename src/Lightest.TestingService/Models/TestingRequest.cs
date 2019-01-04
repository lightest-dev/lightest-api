namespace Lightest.TestingService.Models
{
    public class TestingRequest
    {
        public int UploadId { get; set; }

        public int MemoryLimit { get; set; }

        public int TimeLimit { get; set; }

        public int CheckerId { get; set; }

        public string Type { get; set; }

        public int TestsCount { get; set; }
    }
}
