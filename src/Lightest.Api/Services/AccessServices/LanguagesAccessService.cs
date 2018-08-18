using Lightest.Data.Models;
using Lightest.Data.Models.TaskModels;

namespace Lightest.Api.Services.AccessServices
{
    public class LanguagesAccessService : IAccessService<Language>
    {
        public bool CheckAdminAccess(Language category, ApplicationUser user)
        {
            return true;
        }

        public bool CheckReadAccess(Language category, ApplicationUser user)
        {
            return true;
        }

        public bool CheckWriteAccess(Language category, ApplicationUser user)
        {
            return true;
        }
    }
}
