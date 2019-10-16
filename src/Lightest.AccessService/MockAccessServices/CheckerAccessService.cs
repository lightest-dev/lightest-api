using Lightest.AccessService.Interfaces;
using Lightest.Data.Models;

namespace Lightest.AccessService.MockAccessServices
{
    public class CheckerAccessService : IAccessService<Checker>
    {
        public bool CheckAdminAccess(Checker requested, ApplicationUser requester) => true;

        public bool CheckReadAccess(Checker requested, ApplicationUser requester) => true;

        public bool CheckWriteAccess(Checker requested, ApplicationUser requester) => true;
    }
}
