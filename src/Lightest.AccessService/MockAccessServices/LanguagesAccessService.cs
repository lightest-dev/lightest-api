using Lightest.AccessService.Interfaces;
using Lightest.Data.Models;
using Lightest.Data.Models.TaskModels;

namespace Lightest.AccessService.MockAccessServices
{
    public class LanguagesAccessService : IAccessService<Language>
    {
        public bool HasAdminAccess(ApplicationUser requester) => true;

        public bool HasReadAccess(Language language, ApplicationUser requester) => true;

        public bool HasWriteAccess(Language language, ApplicationUser requester) => true;
    }
}
