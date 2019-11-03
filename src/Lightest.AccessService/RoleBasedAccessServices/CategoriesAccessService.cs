using Lightest.AccessService.Interfaces;
using Lightest.Data.Models;
using Microsoft.AspNetCore.Identity;

namespace Lightest.AccessService.RoleBasedAccessServices
{
    public class CategoriesAccessService : RoleChecker, IAccessService<Category>
    {
        public CategoriesAccessService(UserManager<ApplicationUser> userManager) : base(userManager)
        {
        }

        public bool HasAdminAccess(ApplicationUser requester) => IsAdmin(requester);

        public bool HasReadAccess(Category category, ApplicationUser requester) => true;

        public bool HasWriteAccess(Category category, ApplicationUser requester) => IsTeacherOrAdmin(requester);
    }
}
