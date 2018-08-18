namespace Lightest.Data.Models.TaskModels
{
    public class CheckerResult
    {
        public int FailedTest { get; set; }

        public string Message { get; set; }

        public string Status { get; set; }

        public int SuccessfulTests { get; set; }

        public string Type { get; set; }

        public int UploadId { get; set; }
    }
}
