using Lightest.AccessService.Interfaces;
using Lightest.Data.Models;
using Microsoft.AspNetCore.Identity;

namespace Lightest.AccessService.RoleBasedAccessServices
{
    public class ProfileAccessService : RoleChecker, IAccessService<ApplicationUser>
    {
        public ProfileAccessService(UserManager<ApplicationUser> userManager) : base(userManager)
        {
        }

        public bool CheckAdminAccess(ApplicationUser requested, ApplicationUser requester)
        {
            return IsTeacherOrAdmin(requester);
        }

        public bool CheckReadAccess(ApplicationUser requested, ApplicationUser requester)
        {
            return requester?.Id == requested.Id || IsTeacherOrAdmin(requester);
        }

        public bool CheckWriteAccess(ApplicationUser requested, ApplicationUser requester)
        {
            return requested?.Id == requester.Id || IsTeacherOrAdmin(requester);
        }
    }
}
