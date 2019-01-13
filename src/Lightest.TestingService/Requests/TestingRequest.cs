namespace Lightest.TestingService.Requests
{
    public class TestingRequest : BaseRequest
    {
        public int UploadId { get; set; }

        public int MemoryLimit { get; set; }

        public int TimeLimit { get; set; }

        public int CheckerId { get; set; }

        public int TestsCount { get; set; }

        public override string Type => "upload";
    }
}
