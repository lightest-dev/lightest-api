using Lightest.AccessService.Interfaces;
using Lightest.Data.Models;
using Microsoft.AspNetCore.Identity;

namespace Lightest.AccessService.RoleBasedAccessServices
{
    public class CheckerAccessService : RoleChecker, IAccessService<Checker>
    {
        public CheckerAccessService(UserManager<ApplicationUser> userManager) : base(userManager)
        {
        }

        public bool HasAdminAccess(ApplicationUser requester) => IsAdmin(requester);

        public bool HasReadAccess(Checker requested, ApplicationUser requester) => IsTeacherOrAdmin(requester);

        public bool HasWriteAccess(Checker requested, ApplicationUser requester) => IsTeacherOrAdmin(requester);
    }
}
