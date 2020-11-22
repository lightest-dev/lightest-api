using System;

namespace Lightest.Api.ResponseModels.UploadViews
{
    public class LastUploadView : UploadResultView
    {
        public string Code { get; set; }

        public DateTime UploadDate { get; set; }

        public Guid LanguageId { get; set; }
    }
}
