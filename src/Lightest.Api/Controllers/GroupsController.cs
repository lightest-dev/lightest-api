using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Lightest.AccessService.Interfaces;
using Lightest.Api.Extensions;
using Lightest.Api.Models;
using Lightest.Api.ResponseModels;
using Lightest.Api.ResponseModels.GroupViews;
using Lightest.Data;
using Lightest.Data.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Sieve.Models;
using Sieve.Services;

namespace Lightest.Api.Controllers
{
    [Produces("application/json")]
    [Route("[controller]")]
    public class GroupsController : BaseUserController
    {
        private readonly IAccessService<Group> _accessService;
        private readonly ISieveProcessor _sieveProcessor;
        private readonly IRoleHelper _roleHelper;

        public GroupsController(
            RelationalDbContext context,
            UserManager<ApplicationUser> userManager,
            IAccessService<Group> accessService,
            IRoleHelper roleHelper,
            ISieveProcessor sieveProcessor) : base(context, userManager)
        {
            _accessService = accessService;
            _sieveProcessor = sieveProcessor;
            _roleHelper = roleHelper;
        }

        [HttpGet("all")]
        [ProducesResponseType(200, Type = typeof(IEnumerable<Group>))]
        [ProducesResponseType(403)]
        public async Task<IActionResult> GetGroups([FromQuery]SieveModel sieveModel)
        {
            var user = await GetCurrentUser();
            var groups = _context.Groups.AsNoTracking();

            if (!await _roleHelper.IsAdmin(user))
            {
                return Forbid();
            }

            groups = _sieveProcessor.Apply(sieveModel, groups);

            return Ok(groups);
        }

        [HttpGet]
        [ProducesResponseType(200, Type = typeof(IEnumerable<Group>))]
        public async Task<IEnumerable<ListGroupView>> GetAvailableGroups([FromQuery]SieveModel sieveModel)
        {
            var user = await GetCurrentUser();

            var groups = _context.Groups.AsNoTracking().Include(g => g.Users)
                .Where(g => g.ParentId == null && g.Users.Select(u => u.UserId).Contains(user.Id));

            groups = _sieveProcessor.Apply(sieveModel, groups);

            var result = groups.Select(c => new ListGroupView
            {
                Id = c.Id,
                Name = c.Name,
                Public = c.Public,
                User = c.Users.FirstOrDefault(u => u.UserId == user.Id)
            }).ToList();

            foreach (var group in result)
            {
                if (group.User == null)
                {
                    continue;
                }
                group.CanWrite = group.User.CanWrite;
                group.CanRead = group.User.CanRead;
                group.CanChangeAccess = group.User.CanChangeAccess;
            }

            return result;
        }

        [HttpGet("{id}")]
        [ProducesResponseType(200, Type = typeof(CompleteGroupView))]
        [ProducesResponseType(400)]
        [ProducesResponseType(403)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> GetGroup([FromRoute] Guid id)
        {
            var group = await _context.Groups
                .AsNoTracking()
                .Include(g => g.SubGroups)
                .Include(g => g.Users)
                .ThenInclude(u => u.User)
                .Where(g => g.Id == id)
                .SingleOrDefaultAsync();

            if (group == null)
            {
                return NotFound();
            }

            var user = await GetCurrentUser();

            if (!(await _accessService.CanRead(group.Id, user)))
            {
                return Forbid();
            }

            CompleteGroupView result;

            if (await _accessService.CanEdit(group.Id, user))
            {
                result = new CompleteGroupView
                {
                    Id = group.Id,
                    Name = group.Name,
                    Parent = group.Parent,
                    SubGroups = group.SubGroups,
                    Users = group.Users.Select(u => new AccessRightsUser
                    {
                        Id = u.User.Id,
                        UserName = u.User.UserName,
                        CanRead = u.CanRead,
                        CanWrite = u.CanWrite,
                        CanChangeAccess = u.CanChangeAccess,
                        IsOwner = u.IsOwner
                    })
                };
            }
            else
            {
                result = new CompleteGroupView
                {
                    Id = group.Id,
                    Name = group.Name,
                    Parent = group.Parent,
                    SubGroups = group.SubGroups,
                    Users = null
                };
            }

            return Ok(result);
        }

        [HttpPost]
        [ProducesResponseType(201, Type = typeof(Group))]
        [ProducesResponseType(400)]
        [ProducesResponseType(403)]
        public async Task<IActionResult> PostGroup([FromBody] Group group)
        {
            var user = await GetCurrentUser();

            if (!await _accessService.CanEdit(group.Id, user))
            {
                return Forbid();
            }

            if (group.ParentId != null)
            {
                var parentExists = _context.Groups.Any(g => g.Id == group.ParentId);
                if (!parentExists)
                {
                    return BadRequest(nameof(group.ParentId));
                }
            }

            _context.Groups.Add(group);
            var categoryUser = new UserGroup { UserId = user.Id, GroupId = group.Id };
            categoryUser.SetFullRights();

            group.Users = new List<UserGroup>
            {
                categoryUser
            };

            await _context.SaveChangesAsync();

            return CreatedAtAction("GetGroup", new { id = group.Id }, group);
        }

        [HttpPost("{groupId}/add-users")]
        [ProducesResponseType(200)]
        [ProducesResponseType(403)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> AddUsers([FromRoute] Guid groupId, [FromBody]IEnumerable<AccessRights> users)
        {
            var group = await _context.Groups
                .Include(g => g.Users)
                .SingleOrDefaultAsync(g => g.Id == groupId);
            if (group == null)
            {
                return NotFound();
            }

            var currentUser = await GetCurrentUser();

            if (!await _accessService.CanEdit(group.Id, currentUser))
            {
                return Forbid();
            }

            foreach (var user in users)
            {
                if (!_context.Users.Any(u => u.Id == user.UserId))
                {
                    return BadRequest();
                }

                var categoryUser = group.Users.SingleOrDefault(u => u.UserId == user.UserId);
                //todo: check if user has access to parent group (if parent group is set)
                // if user cannot read parent, he should not have access to child
                if (categoryUser == null)
                {
                    categoryUser = new UserGroup { GroupId = group.Id, UserId = user.UserId };
                    user.CopyTo(categoryUser);
                    group.Users.Add(categoryUser);
                }
                else
                {
                    user.CopyTo(categoryUser);
                }
            }
            await _context.SaveChangesAsync();
            return Ok();
        }

        [HttpPut("{id}")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(403)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> PutGroup([FromRoute] Guid id, [FromBody] Group group)
        {
            if (id != group.Id)
            {
                return BadRequest();
            }

            var dbEntry = await _context.Groups.FindAsync(id);

            if (dbEntry == null)
            {
                return NotFound();
            }

            var user = await GetCurrentUser();

            if (!await _accessService.CanEdit(group.Id, user))
            {
                return Forbid();
            }

            dbEntry.Name = group.Name;
            dbEntry.ParentId = group.ParentId;
            await _context.SaveChangesAsync();
            return Ok();
        }

        [HttpDelete("{id}")]
        [ProducesResponseType(200, Type = typeof(Group))]
        [ProducesResponseType(400)]
        [ProducesResponseType(403)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> DeleteGroup([FromRoute] Guid id)
        {
            var group = await _context.Groups.FindAsync(id);
            if (group == null)
            {
                return NotFound();
            }

            var user = await GetCurrentUser();

            if (!await _accessService.CanEdit(group.Id, user))
            {
                return Forbid();
            }

            _context.Groups.Remove(group);
            await _context.SaveChangesAsync();

            return Ok(group);
        }
    }
}
