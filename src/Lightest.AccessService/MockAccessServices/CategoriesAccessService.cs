using Lightest.AccessService.Interfaces;
using Lightest.Data.Models;

namespace Lightest.AccessService.MockAccessServices
{
    public class CategoriesAccessService : IAccessService<Category>
    {
        public bool CheckAdminAccess(Category category, ApplicationUser requester) => true;

        public bool CheckReadAccess(Category category, ApplicationUser requester) => true;

        public bool CheckWriteAccess(Category category, ApplicationUser requester) => true;
    }
}
