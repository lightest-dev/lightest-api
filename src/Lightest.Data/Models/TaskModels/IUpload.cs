namespace Lightest.Data.Models.TaskModels
{
    public interface IUpload
    {
        string Message { get; set; }

        double Points { get; set; }

        string Status { get; set; }

        bool TestingFinished { get; set; }

        int UploadId { get; set; }

        ApplicationUser User { get; set; }

        string UserId { get; set; }
    }
}
