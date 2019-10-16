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

        public bool CheckAdminAccess(Category category, ApplicationUser requester) => IsAdmin(requester);

        public bool CheckReadAccess(Category category, ApplicationUser requester) => true;

        public bool CheckWriteAccess(Category category, ApplicationUser requester) => IsTeacherOrAdmin(requester);
    }
}
