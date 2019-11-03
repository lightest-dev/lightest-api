using Lightest.Data.Models;

namespace Lightest.AccessService.Interfaces
{
    public interface IAccessService<in T>
    {
        bool HasAdminAccess(ApplicationUser requester);

        bool HasReadAccess(T requested, ApplicationUser requester);

        bool HasWriteAccess(T requested, ApplicationUser requester);
    }
}
