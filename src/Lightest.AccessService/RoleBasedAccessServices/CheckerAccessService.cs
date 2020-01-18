using System;
using System.Threading.Tasks;
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

        public bool HasAdminAccess(ApplicationUser requester) => IsAdmin(requester).GetAwaiter().GetResult();

        public Task<bool> HasReadAccess(Guid id, ApplicationUser requester) => IsTeacherOrAdmin(requester);

        public Task<bool> HasWriteAccess(Guid id, ApplicationUser requester) => IsTeacherOrAdmin(requester);
    }
}
