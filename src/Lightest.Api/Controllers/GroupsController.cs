using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Lightest.AccessService.Interfaces;
using Lightest.Api.Extensions;
using Lightest.Api.Models;
using Lightest.Api.ResponseModels;
using Lightest.Data;
using Lightest.Data.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Lightest.Api.Controllers
{
    [Produces("application/json")]
    [Route("[controller]")]
    [Authorize]
    public class GroupsController : BaseUserController
    {
        private readonly IAccessService<Group> _accessService;

        public GroupsController(
            RelationalDbContext context,
            IAccessService<Group> accessService,
            UserManager<ApplicationUser> userManager) : base(context, userManager)
        {
            _accessService = accessService;
        }

        // GET: api/Groups
        [HttpGet]
        public async Task<IEnumerable<Group>> GetGroups()
        {
            var user = await GetCurrentUser();
            //todo: check if admin and return all
            return _context.Groups
                .AsNoTracking()
                .Include(g => g.Users)
                .Where(g => g.Users.Select(u => u.UserId).Contains(user.Id));
        }

        // GET: api/Groups/5
        [HttpGet("{id}")]
        [ProducesResponseType(200, Type = typeof(CompleteGroup))]
        [ProducesResponseType(400)]
        [ProducesResponseType(403)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> GetGroup([FromRoute] int id)
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

            if (!_accessService.CheckReadAccess(group, user))
            {
                return Forbid();
            }

            CompleteGroup result;

            if (_accessService.CheckWriteAccess(group, user))
            {
                result = new CompleteGroup
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
                result = new CompleteGroup
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

        // POST: api/Groups
        [HttpPost]
        [ProducesResponseType(201, Type = typeof(Group))]
        [ProducesResponseType(400)]
        [ProducesResponseType(403)]
        public async Task<IActionResult> PostGroup([FromBody] Group group)
        {
            var user = await GetCurrentUser();

            if (!_accessService.CheckWriteAccess(group, user))
            {
                return Forbid();
            }

            _context.Groups.Add(group);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetGroup", new { id = group.Id }, group);
        }

        [HttpPost("{groupId}/add-users")]
        [ProducesResponseType(200)]
        [ProducesResponseType(403)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> AddUsers([FromRoute] int groupId, [FromBody]IEnumerable<AccessRights> users)
        {
            var group = await _context.Groups.FindAsync(groupId);
            if (group == null)
            {
                return NotFound();
            }

            var currentUser = await GetCurrentUser();

            if (!_accessService.CheckWriteAccess(group, currentUser))
            {
                return Forbid();
            }

            foreach (var user in users)
            {
                if (!UserExists(user.UserId))
                {
                    continue;
                }
                var userGroup = new UserGroup { GroupId = group.Id, UserId = user.UserId };
                user.CopyTo(userGroup);
                group.Users.Add(userGroup);
            }
            await _context.SaveChangesAsync();
            return Ok();
        }

        // PUT: api/Groups/5
        [HttpPut("{id}")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(403)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> PutGroup([FromRoute] int id, [FromBody] Group group)
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

            if (!_accessService.CheckWriteAccess(group, user))
            {
                return Forbid();
            }

            dbEntry.Name = group.Name;
            dbEntry.ParentId = group.ParentId;
            await _context.SaveChangesAsync();
            return Ok();
        }

        // DELETE: api/Groups/5
        [HttpDelete("{id}")]
        [ProducesResponseType(200, Type = typeof(Group))]
        [ProducesResponseType(400)]
        [ProducesResponseType(403)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> DeleteGroup([FromRoute] int id)
        {
            var group = await _context.Groups.FindAsync(id);
            if (group == null)
            {
                return NotFound();
            }

            var user = await GetCurrentUser();

            if (!_accessService.CheckWriteAccess(group, user))
            {
                return Forbid();
            }

            _context.Groups.Remove(group);
            await _context.SaveChangesAsync();

            return Ok(group);
        }

        private bool UserExists(string id)
        {
            return _context.Users.Any(u => u.Id == id);
        }
    }
}
