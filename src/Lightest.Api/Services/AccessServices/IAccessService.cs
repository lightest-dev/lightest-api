using Lightest.Data.Models;

namespace Lightest.Api.Services.AccessServices
{
    public interface IAccessService<in T>
    {
        bool CheckAdminAccess(T category, ApplicationUser user);

        bool CheckReadAccess(T category, ApplicationUser user);

        bool CheckWriteAccess(T category, ApplicationUser user);
    }
}
