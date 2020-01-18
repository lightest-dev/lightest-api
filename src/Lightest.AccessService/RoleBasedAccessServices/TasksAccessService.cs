using System;
using System.Linq;
using System.Threading.Tasks;
using Lightest.AccessService.Interfaces;
using Lightest.Data;
using Lightest.Data.Models;
using Lightest.Data.Models.TaskModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Lightest.AccessService.RoleBasedAccessServices
{
    public class TasksAccessService : RoleChecker, IAccessService<TaskDefinition>
    {
        private readonly RelationalDbContext _context;

        public TasksAccessService(RelationalDbContext context, UserManager<ApplicationUser> userManager) : base(userManager)
        {
            _context = context;
        }

        public bool HasAdminAccess(ApplicationUser requester) => IsAdmin(requester).GetAwaiter().GetResult();

        public async Task<bool> HasReadAccess(Guid id, ApplicationUser requester)
        {
            var userExists = _context.Tasks.Include(t => t.Users)
                .Any(t => t.Id == id
                    && (t.Public || t.Users.Any(u => u.UserId == requester.Id)));
            return userExists || await IsTeacherOrAdmin(requester);
        }

        public Task<bool> HasWriteAccess(Guid id, ApplicationUser requester) => IsTeacherOrAdmin(requester);
    }
}
