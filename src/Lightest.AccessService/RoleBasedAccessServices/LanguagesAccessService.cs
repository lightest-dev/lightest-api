using System;
using System.Threading.Tasks;
using Lightest.AccessService.Interfaces;
using Lightest.Data.Models;
using Lightest.Data.Models.TaskModels;
using Microsoft.AspNetCore.Identity;

namespace Lightest.AccessService.RoleBasedAccessServices
{
    public class LanguagesAccessService : RoleChecker, IAccessService<Language>
    {
        public LanguagesAccessService(UserManager<ApplicationUser> userManager) : base(userManager)
        {
        }

        public bool HasAdminAccess(ApplicationUser requester) => IsAdmin(requester).GetAwaiter().GetResult();

        public Task<bool> HasReadAccess(Guid id, ApplicationUser requester) => Task.FromResult(true);

        public Task<bool> HasWriteAccess(Guid id, ApplicationUser requester) => IsTeacherOrAdmin(requester);
    }
}
