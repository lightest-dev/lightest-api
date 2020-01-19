﻿using System;
using System.Linq;
using System.Threading.Tasks;
using Lightest.AccessService.Interfaces;
using Lightest.Data;
using Lightest.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace Lightest.AccessService.RoleBasedAccessServices
{
    internal class GroupsAccessService : BaseAccessService, IAccessService<Group>
    {
        private readonly RelationalDbContext _context;

        public GroupsAccessService(RelationalDbContext context, IRoleHelper roleHelper) : base(roleHelper)
        {
            _context = context;
        }

        public async Task<bool> HasReadAccess(Guid id, ApplicationUser requester)
        {
            var userExists = _context.Groups.Include(g => g.Users)
                .Any(g => g.Id == id
                    && (g.Public || g.Users.Any(u => u.UserId == requester.Id)));
            return userExists || await IsTeacher(requester);
        }

        public Task<bool> HasWriteAccess(Guid id, ApplicationUser requester) => IsTeacher(requester);
    }
}
