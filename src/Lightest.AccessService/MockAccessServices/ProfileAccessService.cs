using System;
using System.Threading.Tasks;
using Lightest.AccessService.Interfaces;
using Lightest.Data.Models;

namespace Lightest.AccessService.MockAccessServices
{
    internal class ProfileAccessService : IAccessService<ApplicationUser>
    {
        public Task<bool> CanAdd(ApplicationUser item, ApplicationUser requester) => Task.FromResult(true);

        public Task<bool> CanRead(Guid id, ApplicationUser requester) => Task.FromResult(true);

        public Task<bool> CanEdit(Guid id, ApplicationUser requester) => Task.FromResult(true);
    }
}
