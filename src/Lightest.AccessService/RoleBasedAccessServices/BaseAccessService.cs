using System.Threading.Tasks;
using Lightest.AccessService.Interfaces;
using Lightest.Data.Models;

namespace Lightest.AccessService.RoleBasedAccessServices
{
    internal abstract class BaseAccessService
    {
        private readonly IRoleHelper _roleHelper;

        protected BaseAccessService(IRoleHelper roleHelper)
        {
            _roleHelper = roleHelper;
        }

        protected Task<bool> IsAdmin(ApplicationUser user) => _roleHelper.IsAdmin(user);

        protected Task<bool> IsTeacher(ApplicationUser user) => _roleHelper.IsTeacher(user);
    }
}
