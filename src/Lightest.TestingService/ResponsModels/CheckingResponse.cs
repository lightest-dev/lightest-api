using System;

namespace Lightest.TestingService.ResponsModels
{
    public class CheckingResponse
    {
        public Guid UploadId { get; set; }

        public string Type { get; set; }

        public string Message { get; set; }

        public string Status { get; set; }

        public int SuccessfulTests { get; set; }

        public int FailedTest { get; set; }

        public string Ip { get; set; }
    }
}
