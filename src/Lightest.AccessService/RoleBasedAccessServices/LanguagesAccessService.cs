using System;
using System.Threading.Tasks;
using Lightest.AccessService.Interfaces;
using Lightest.Data.Models;
using Lightest.Data.Models.TaskModels;

namespace Lightest.AccessService.RoleBasedAccessServices
{
    internal class LanguagesAccessService : BaseAccessService, IAccessService<Language>
    {
        public LanguagesAccessService(IRoleHelper roleHelper) : base(roleHelper)
        {
        }

        public Task<bool> CanRead(Guid id, ApplicationUser requester) => Task.FromResult(true);

        public Task<bool> CanWrite(Guid id, ApplicationUser requester) => IsTeacher(requester);
    }
}
