using System;
using System.Linq;
using System.Threading.Tasks;
using Lightest.AccessService.Interfaces;
using Lightest.Data;
using Lightest.Data.Models;
using Lightest.Data.Models.TaskModels;
using Microsoft.EntityFrameworkCore;

namespace Lightest.AccessService.RoleBasedAccessServices
{
    internal class TasksAccessService : BaseAccessService, IAccessService<TaskDefinition>
    {
        private readonly RelationalDbContext _context;

        public TasksAccessService(RelationalDbContext context, IRoleHelper roleHelper) : base(roleHelper)
        {
            _context = context;
        }

        public bool HasAdminAccess(ApplicationUser requester) => IsAdmin(requester).GetAwaiter().GetResult();

        public async Task<bool> CanRead(Guid id, ApplicationUser requester)
        {
            var userExists = _context.Tasks.Include(t => t.Users)
                .Any(t => t.Id == id
                    && (t.Public || t.Users.Any(u => u.UserId == requester.Id)));
            return userExists || await IsTeacher(requester);
        }

        public Task<bool> CanEdit(Guid id, ApplicationUser requester) => IsTeacher(requester);

        public Task<bool> CanAdd(TaskDefinition item, ApplicationUser requester) => IsTeacher(requester);
    }
}
