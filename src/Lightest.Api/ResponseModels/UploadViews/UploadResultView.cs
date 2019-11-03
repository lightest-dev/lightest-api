using System;

namespace Lightest.Api.ResponseModels.UploadViews
{
    public class UploadResultView
    {
        public Guid Id { get; set; }

        public string Status { get; set; }

        public string Message { get; set; }

        public double Points { get; set; }
    }
}
