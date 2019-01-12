using Lightest.AccessService.Interfaces;
using Lightest.Data.Models;

namespace Lightest.AccessService.RoleBasedAccessServices
{
    public class GroupsAccessService : IAccessService<Group>
    {
        public bool CheckAdminAccess(Group group, ApplicationUser requester)
        {
            return true;
        }

        public bool CheckReadAccess(Group group, ApplicationUser requester)
        {
            return true;
        }

        public bool CheckWriteAccess(Group group, ApplicationUser requester)
        {
            return true;
        }
    }
}
