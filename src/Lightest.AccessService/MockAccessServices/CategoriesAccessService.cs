using System;
using System.Threading.Tasks;
using Lightest.AccessService.Interfaces;
using Lightest.Data.Models;

namespace Lightest.AccessService.MockAccessServices
{
    internal class CategoriesAccessService : IAccessService<Category>
    {
        public Task<bool> HasReadAccess(Guid id, ApplicationUser requester) => Task.FromResult(true);

        public Task<bool> HasWriteAccess(Guid id, ApplicationUser requester) => Task.FromResult(true);
    }
}
