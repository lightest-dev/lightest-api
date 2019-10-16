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

        public bool CheckAdminAccess(Checker requested, ApplicationUser requester) => IsAdmin(requester);

        public bool CheckReadAccess(Checker requested, ApplicationUser requester) => IsTeacherOrAdmin(requester);

        public bool CheckWriteAccess(Checker requested, ApplicationUser requester) => IsTeacherOrAdmin(requester);
    }
}
