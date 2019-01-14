using System;

namespace Lightest.Data.Models.TaskModels
{
    public interface IUpload
    {
        Guid UploadId { get; set; }

        string Message { get; set; }

        double Points { get; set; }

        string Status { get; set; }

        bool TestingFinished { get; set; }

        ApplicationUser User { get; set; }

        string UserId { get; set; }

        TaskDefinition Task { get; set; }

        Guid TaskId { get; set; }
    }
}
