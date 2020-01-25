using System;
using System.Threading.Tasks;
using Lightest.AccessService.Interfaces;
using Lightest.Data.Models;

namespace Lightest.AccessService.MockAccessServices
{
    internal class CategoriesAccessService : IAccessService<Category>
    {
        public Task<bool> CanAdd(Category item, ApplicationUser requester) => Task.FromResult(true);

        public Task<bool> CanRead(Guid id, ApplicationUser requester) => Task.FromResult(true);

        public Task<bool> CanWrite(Guid id, ApplicationUser requester) => Task.FromResult(true);
    }
}
