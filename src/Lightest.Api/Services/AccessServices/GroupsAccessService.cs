using Lightest.Data.Models;

namespace Lightest.Api.Services.AccessServices
{
    public class GroupsAccessService : IAccessService<Group>
    {
        public bool CheckAdminAccess(Group category, ApplicationUser user)
        {
            return true;
        }

        public bool CheckReadAccess(Group category, ApplicationUser user)
        {
            return true;
        }

        public bool CheckWriteAccess(Group category, ApplicationUser user)
        {
            return true;
        }
    }
}
