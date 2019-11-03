using System.Linq;
using Lightest.AccessService.Interfaces;
using Lightest.Data.Models;
using Microsoft.AspNetCore.Identity;

namespace Lightest.AccessService.RoleBasedAccessServices
{
    public class GroupsAccessService : RoleChecker, IAccessService<Group>
    {
        public GroupsAccessService(UserManager<ApplicationUser> userManager) : base(userManager)
        {
        }

        public bool HasAdminAccess(ApplicationUser requester) => IsAdmin(requester);

        public bool HasReadAccess(Group group, ApplicationUser requester) => group?.Users?.Any(u => u.UserId == requester.Id) == true
                           || IsTeacherOrAdmin(requester) || group?.Public == true;

        public bool HasWriteAccess(Group group, ApplicationUser requester) => IsTeacherOrAdmin(requester);
    }
}
