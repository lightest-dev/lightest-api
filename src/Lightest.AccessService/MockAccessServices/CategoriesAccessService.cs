using Lightest.AccessService.Interfaces;
using Lightest.Data.Models;

namespace Lightest.AccessService.MockAccessServices
{
    public class CategoriesAccessService : IAccessService<Category>
    {
        public bool HasAdminAccess(ApplicationUser requester) => true;

        public bool HasReadAccess(Category category, ApplicationUser requester) => true;

        public bool HasWriteAccess(Category category, ApplicationUser requester) => true;
    }
}
