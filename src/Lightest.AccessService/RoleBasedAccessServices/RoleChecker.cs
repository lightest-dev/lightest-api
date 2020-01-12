using System.Threading.Tasks;
using Lightest.Data.Models;
using Microsoft.AspNetCore.Identity;

namespace Lightest.AccessService.RoleBasedAccessServices
{
    public class RoleChecker
    {
        protected readonly UserManager<ApplicationUser> _userManager;

        protected RoleChecker(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }

        protected Task<bool> IsAdmin(ApplicationUser user) => _userManager.IsInRoleAsync(user, "Admin");

        protected Task<bool> IsTeacher(ApplicationUser user) => _userManager.IsInRoleAsync(user, "Teacher");

        protected async Task<bool> IsTeacherOrAdmin(ApplicationUser user) => await IsTeacher(user) || await IsAdmin(user);
    }
}
