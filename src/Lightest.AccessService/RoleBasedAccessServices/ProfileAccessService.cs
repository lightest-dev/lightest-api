using System;
using System.Threading.Tasks;
using Lightest.AccessService.Interfaces;
using Lightest.Data.Models;

namespace Lightest.AccessService.RoleBasedAccessServices
{
    internal class ProfileAccessService : BaseAccessService, IAccessService<ApplicationUser>
    {
        public ProfileAccessService(IRoleHelper roleHelper) : base(roleHelper)
        {
        }

        public Task<bool> CanAdd(ApplicationUser item, ApplicationUser requester) => Task.FromResult(false);

        public async Task<bool> CanRead(Guid id, ApplicationUser requester)
        {
            var userId = id.ToString();
            var sameUser = userId == requester.Id;
            return sameUser || await IsTeacher(requester);
        }

        public async Task<bool> CanEdit(Guid id, ApplicationUser requester) => id.ToString() == requester.Id || await IsTeacher(requester);
    }
}
