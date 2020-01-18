using System;
using System.Threading.Tasks;
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

        public bool HasAdminAccess(ApplicationUser requester) => IsTeacherOrAdmin(requester).GetAwaiter().GetResult();

        public async Task<bool> HasReadAccess(Guid id, ApplicationUser requester)
        {
            var userId = id.ToString();
            var sameUser = userId == requester.Id;
            return sameUser || await IsTeacherOrAdmin(requester);
        }

        public async Task<bool> HasWriteAccess(Guid id, ApplicationUser requester) => id.ToString() == requester.Id || await IsTeacherOrAdmin(requester);
    }
}
