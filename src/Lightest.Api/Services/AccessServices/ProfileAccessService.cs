using Lightest.Data.Models;

namespace Lightest.Api.Services.AccessServices
{
    public class ProfileAccessService : IAccessService<ApplicationUser>
    {
        public bool CheckAdminAccess(ApplicationUser requested, ApplicationUser requester)
        {
            return true;
        }

        public bool CheckReadAccess(ApplicationUser requested, ApplicationUser requester)
        {
            return true;
        }

        public bool CheckWriteAccess(ApplicationUser requested, ApplicationUser requester)
        {
            return true;
        }
    }
}
