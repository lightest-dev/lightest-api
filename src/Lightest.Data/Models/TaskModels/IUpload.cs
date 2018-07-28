namespace Lightest.Data.Models.TaskModels
{
    public interface IUpload
    {
        int UploadId { get; set; }

        double Points { get; set; }

        string Status { get; set; }

        string Message { get; set; }

        bool TestingFinished { get; set; }

        string UserId { get; set; }

        ApplicationUser User { get; set; }
    }
}