using System.Threading.Tasks;
using Lightest.AccessService.Interfaces;
using Lightest.Data.Models;
using Microsoft.AspNetCore.Identity;

namespace Lightest.AccessService
{
    internal class RoleHelper : IRoleHelper
    {
        public readonly UserManager<ApplicationUser> _userManager;

        public RoleHelper(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }

        public Task<bool> IsAdmin(ApplicationUser user) => _userManager.IsInRoleAsync(user, "Admin");

        public async Task<bool> IsTeacher(ApplicationUser user) =>
            await IsAdmin(user) || await _userManager.IsInRoleAsync(user, "Teacher");
    }
}
