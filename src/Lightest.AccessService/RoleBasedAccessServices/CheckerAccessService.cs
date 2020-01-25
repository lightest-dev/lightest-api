using System;
using System.Threading.Tasks;
using Lightest.AccessService.Interfaces;
using Lightest.Data.Models;

namespace Lightest.AccessService.RoleBasedAccessServices
{
    internal class CheckerAccessService : BaseAccessService, IAccessService<Checker>
    {
        public CheckerAccessService(IRoleHelper roleHelper) : base(roleHelper)
        {
        }

        public Task<bool> HasReadAccess(Guid id, ApplicationUser requester) => IsTeacher(requester);

        public Task<bool> HasWriteAccess(Guid id, ApplicationUser requester) => IsTeacher(requester);
    }
}
