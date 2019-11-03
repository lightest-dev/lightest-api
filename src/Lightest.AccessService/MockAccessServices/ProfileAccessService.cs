using Lightest.AccessService.Interfaces;
using Lightest.Data.Models;

namespace Lightest.AccessService.MockAccessServices
{
    public class ProfileAccessService : IAccessService<ApplicationUser>
    {
        public bool HasAdminAccess(ApplicationUser requester) => true;

        public bool HasReadAccess(ApplicationUser requested, ApplicationUser requester) => true;

        public bool HasWriteAccess(ApplicationUser requested, ApplicationUser requester) => true;
    }
}
