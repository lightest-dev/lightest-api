using Lightest.AccessService.Interfaces;
using Lightest.Data.Models;
using Lightest.Data.Models.TaskModels;

namespace Lightest.AccessService.MockAccessServices
{
    public class LanguagesAccessService : IAccessService<Language>
    {
        public bool CheckAdminAccess(Language language, ApplicationUser requester)
        {
            return true;
        }

        public bool CheckReadAccess(Language language, ApplicationUser requester)
        {
            return true;
        }

        public bool CheckWriteAccess(Language language, ApplicationUser requester)
        {
            return true;
        }
    }
}
