using Lightest.Data.Models;

namespace Lightest.Api.Services.AccessServices
{
    public class CategoriesAccessService : IAccessService<Category>
    {
        public bool CheckAdminAccess(Category category, ApplicationUser requester)
        {
            return true;
        }

        public bool CheckReadAccess(Category category, ApplicationUser requester)
        {
            return true;
        }

        public bool CheckWriteAccess(Category category, ApplicationUser requester)
        {
            return true;
        }
    }
}
