using System.Security.Claims;
using IdentityServer4.Extensions;
using IdentityServer4.Models;
using IdentityServer4.Services;
using Lightest.Data.Models;
using Microsoft.AspNetCore.Identity;

namespace Lightest.IdentityServer.Services
{
    public class ProfileService : IProfileService
    {
        private readonly UserManager<ApplicationUser> _userManager;

        public ProfileService(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }

        public async System.Threading.Tasks.Task GetProfileDataAsync(ProfileDataRequestContext context)
        {
            var sub = context.Subject.GetSubjectId();
            var user = await _userManager.FindByIdAsync(sub);
            context.IssuedClaims.Add(new Claim("Admin", (await _userManager.IsInRoleAsync(user, "Admin")).ToString()));
            context.IssuedClaims.Add(new Claim("Teacher", (await _userManager.IsInRoleAsync(user, "Teacher")).ToString()));
        }

        public System.Threading.Tasks.Task IsActiveAsync(IsActiveContext context)
        {
            return System.Threading.Tasks.Task.Run(() =>
            {
                context.IsActive = true;
            });
        }
    }
}
