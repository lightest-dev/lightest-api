using Lightest.Data.Models;

namespace Lightest.Api.Services.AccessServices
{
    public interface IAccessService<in T>
    {
        bool CheckAdminAccess(T requested, ApplicationUser requester);

        bool CheckReadAccess(T requested, ApplicationUser requester);

        bool CheckWriteAccess(T requested, ApplicationUser requester);
    }
}
