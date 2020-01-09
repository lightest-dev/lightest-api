using Lightest.AccessService.Interfaces;
using Lightest.Data.Models;

namespace Lightest.AccessService.MockAccessServices
{
    public class GroupsAccessService : IAccessService<Group>
    {
        public bool HasAdminAccess(ApplicationUser requester) => true;

        public bool HasReadAccess(Group group, ApplicationUser requester) => true;

        public bool HasWriteAccess(Group group, ApplicationUser requester) => true;
    }
}
