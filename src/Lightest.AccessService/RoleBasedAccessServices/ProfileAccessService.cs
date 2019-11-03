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

        public bool HasAdminAccess(ApplicationUser requester) => IsTeacherOrAdmin(requester);

        public bool HasReadAccess(ApplicationUser requested, ApplicationUser requester) => requester?.Id == requested.Id || IsTeacherOrAdmin(requester);

        public bool HasWriteAccess(ApplicationUser requested, ApplicationUser requester) => requested?.Id == requester.Id || IsTeacherOrAdmin(requester);
    }
}
