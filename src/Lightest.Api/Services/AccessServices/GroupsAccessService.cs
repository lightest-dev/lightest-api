using Lightest.Data.Models;

namespace Lightest.Api.Services.AccessServices
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
