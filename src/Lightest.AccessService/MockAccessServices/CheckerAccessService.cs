using Lightest.AccessService.Interfaces;
using Lightest.Data.Models;

namespace Lightest.AccessService.MockAccessServices
{
    public class CheckerAccessService : IAccessService<Checker>
    {
        public bool HasAdminAccess(ApplicationUser requester) => true;

        public bool HasReadAccess(Checker requested, ApplicationUser requester) => true;

        public bool HasWriteAccess(Checker requested, ApplicationUser requester) => true;
    }
}
