namespace Lightest.Data.Models.TaskModels
{
    public interface IAccessRights
    {
        string UserId { get; set; }

        ApplicationUser User { get; set; }

        bool CanRead { get; set; }

        bool CanWrite { get; set; }

        bool CanChangeAccess { get; set; }

        bool IsOwner { get; set; }
    }
}