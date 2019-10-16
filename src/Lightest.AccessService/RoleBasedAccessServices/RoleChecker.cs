using Lightest.Data.Models;
using Microsoft.AspNetCore.Identity;

namespace Lightest.AccessService.RoleBasedAccessServices
{
    public abstract class RoleChecker
    {
        protected readonly UserManager<ApplicationUser> _userManager;

        protected RoleChecker(UserManager<ApplicationUser> userManager) => _userManager = userManager;

        protected bool IsAdmin(ApplicationUser user) => _userManager.IsInRoleAsync(user, "Admin")
                .ConfigureAwait(false).GetAwaiter().GetResult();

        protected bool IsTeacher(ApplicationUser user) => _userManager.IsInRoleAsync(user, "Teacher")
                .ConfigureAwait(false).GetAwaiter().GetResult();

        protected bool IsTeacherOrAdmin(ApplicationUser user) => IsTeacher(user) || IsAdmin(user);
    }
}
