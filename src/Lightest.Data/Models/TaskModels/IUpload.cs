namespace Lightest.Data.Models.TaskModels
{
    public interface IUpload
    {
        double Points { get; set; }

        string Status { get; set; }

        string UserId { get; set; }

        ApplicationUser User { get; set; }
    }
}