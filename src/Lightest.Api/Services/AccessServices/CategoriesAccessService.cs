using Lightest.Data.Models;

namespace Lightest.Api.Services.AccessServices
{
    public class CategoriesAccessService : IAccessService<Category>
    {
        public bool CheckAdminAccess(Category category, ApplicationUser user)
        {
            return true;
        }

        public bool CheckReadAccess(Category category, ApplicationUser user)
        {
            return true;
        }

        public bool CheckWriteAccess(Category category, ApplicationUser user)
        {
            return true;
        }
    }
}
