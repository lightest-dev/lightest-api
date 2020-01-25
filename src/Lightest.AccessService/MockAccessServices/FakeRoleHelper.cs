using System.Threading.Tasks;
using Lightest.AccessService.Interfaces;
using Lightest.Data.Models;

namespace Lightest.AccessService.MockAccessServices
{
    internal class FakeRoleHelper : IRoleHelper
    {
        public Task<bool> IsAdmin(ApplicationUser user) => Task.FromResult(true);

        public Task<bool> IsTeacher(ApplicationUser user) => Task.FromResult(true);
    }
}
