namespace Lightest.Data.Models.TaskModels
{
    public interface IAccessRights
    {
        bool CanRead { get; set; }

        bool CanWrite { get; set; }

        bool CanChangeAccess { get; set; }

        bool IsOwner { get; set; }

        ApplicationUser User { get; set; }

        string UserId { get; set; }
    }
}
