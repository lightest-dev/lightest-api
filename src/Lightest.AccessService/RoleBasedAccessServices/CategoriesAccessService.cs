using System;
using System.Threading.Tasks;
using Lightest.AccessService.Interfaces;
using Lightest.Data.Models;

namespace Lightest.AccessService.RoleBasedAccessServices
{
    internal class CategoriesAccessService : BaseAccessService, IAccessService<Category>
    {
        public CategoriesAccessService(IRoleHelper roleHelper) : base(roleHelper)
        {
        }

        public Task<bool> HasReadAccess(Guid id, ApplicationUser requester) => Task.FromResult(true);

        public Task<bool> HasWriteAccess(Guid id, ApplicationUser requester) => IsTeacher(requester);
    }
}
