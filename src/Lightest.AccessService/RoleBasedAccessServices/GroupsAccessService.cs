using System;
using System.Linq;
using System.Threading.Tasks;
using Lightest.AccessService.Interfaces;
using Lightest.Data;
using Lightest.Data.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Lightest.AccessService.RoleBasedAccessServices
{
    public class GroupsAccessService : RoleChecker, IAccessService<Group>
    {
        private readonly RelationalDbContext _context;

        public GroupsAccessService(RelationalDbContext context, UserManager<ApplicationUser> userManager) : base(userManager)
        {
            _context = context;
        }

        public bool HasAdminAccess(ApplicationUser requester) => IsAdmin(requester).GetAwaiter().GetResult();

        public async Task<bool> HasReadAccess(Guid id, ApplicationUser requester)
        {
            var userExists = _context.Groups.Include(g => g.Users)
                .Any(g => g.Id == id
                    && (g.Public || g.Users.Any(u => u.UserId == requester.Id)));
            return userExists || await IsTeacherOrAdmin(requester);
        }

        public bool HasWriteAccess(Group group, ApplicationUser requester) => IsTeacherOrAdmin(requester).GetAwaiter().GetResult();
    }
}
