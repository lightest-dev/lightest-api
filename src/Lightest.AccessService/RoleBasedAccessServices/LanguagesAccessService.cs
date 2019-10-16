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

        public bool CheckAdminAccess(Language language, ApplicationUser requester) => IsAdmin(requester);

        public bool CheckReadAccess(Language language, ApplicationUser requester) => true;

        public bool CheckWriteAccess(Language language, ApplicationUser requester) => IsTeacherOrAdmin(requester);
    }
}
