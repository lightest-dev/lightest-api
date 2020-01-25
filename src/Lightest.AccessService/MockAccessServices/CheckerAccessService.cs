using System;
using System.Threading.Tasks;
using Lightest.AccessService.Interfaces;
using Lightest.Data.Models;

namespace Lightest.AccessService.MockAccessServices
{
    internal class CheckerAccessService : IAccessService<Checker>
    {
        public Task<bool> CanAdd(Checker item, ApplicationUser requester) => Task.FromResult(true);

        public Task<bool> CanRead(Guid id, ApplicationUser requester) => Task.FromResult(true);

        public Task<bool> CanWrite(Guid id, ApplicationUser requester) => Task.FromResult(true);
    }
}
