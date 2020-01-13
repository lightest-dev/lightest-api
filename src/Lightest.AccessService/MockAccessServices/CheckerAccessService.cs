using System;
using System.Threading.Tasks;
using Lightest.AccessService.Interfaces;
using Lightest.Data.Models;

namespace Lightest.AccessService.MockAccessServices
{
    public class CheckerAccessService : IAccessService<Checker>
    {
        public bool HasAdminAccess(ApplicationUser requester) => true;

        public Task<bool> HasReadAccess(Guid id, ApplicationUser requester) => Task.FromResult(true);

        public bool HasWriteAccess(Checker requested, ApplicationUser requester) => true;
    }
}
