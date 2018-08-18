using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Lightest.Api.Extensions;
using Lightest.Api.Services.AccessServices;
using Lightest.Api.ViewModels;
using Lightest.Data;
using Lightest.Data.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Lightest.Api.Controllers
{
    [Produces("application/json")]
    [Route("api/[controller]")]
    [Authorize]
    public class GroupsController : Controller
    {
        private readonly IAccessService<Group> _accessService;
        private readonly RelationalDbContext _context;

        public GroupsController(RelationalDbContext context, IAccessService<Group> accessService)
        {
            _context = context;
            _accessService = accessService;
        }

        [HttpPost("{groupId}/add-user")]
        [ProducesResponseType(200)]
        [ProducesResponseType(403)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> AddUser([FromRoute] int groupId, [FromBody]AccessRightsViewModel user)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (!UserExists(user.UserId))
            {
                return NotFound(nameof(user));
            }

            var group = await _context.Groups.Include(g => g.Users).SingleOrDefaultAsync(g => g.Id == groupId);

            if (group == null)
            {
                return NotFound(nameof(group));
            }

            var currentUser = GetCurrentUser();

            if (!_accessService.CheckWriteAccess(group, currentUser))
            {
                return Forbid();
            }

            var userGroup = new UserGroup { GroupId = group.Id, UserId = user.UserId };
            user.CopyTo(userGroup);

            group.Users.Add(userGroup);
            await _context.SaveChangesAsync();
            return Ok();
        }

        [HttpPost("{groupId}/add-users")]
        [ProducesResponseType(200)]
        [ProducesResponseType(403)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> AddUsers([FromRoute] int groupId, [FromBody]IEnumerable<AccessRightsViewModel> users)
        {
            var group = await _context.Groups.FindAsync(groupId);
            if (group == null)
            {
                return NotFound();
            }

            var currentUser = GetCurrentUser();

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

        // DELETE: api/Groups/5
        [HttpDelete("{id}")]
        [ProducesResponseType(200, Type = typeof(Group))]
        [ProducesResponseType(400)]
        [ProducesResponseType(403)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> DeleteGroup([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var group = await _context.Groups.FindAsync(id);
            if (group == null)
            {
                return NotFound();
            }

            var user = GetCurrentUser();

            if (!_accessService.CheckWriteAccess(group, user))
            {
                return Forbid();
            }

            _context.Groups.Remove(group);
            await _context.SaveChangesAsync();

            return Ok(group);
        }

        // GET: api/Groups/5
        [HttpGet("{id}")]
        [ProducesResponseType(200, Type = typeof(CompleteGroupViewModel))]
        [ProducesResponseType(400)]
        [ProducesResponseType(403)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> GetGroup([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

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

            var user = GetCurrentUser();

            if (!_accessService.CheckReadAccess(group, user))
            {
                return Forbid();
            }

            var result = new CompleteGroupViewModel
            {
                Id = group.Id,
                Name = group.Name,
                Parent = group.Parent,
                SubGroups = group.SubGroups,
                Users = group.Users.Select(u => new AccessRightsUserViewModel
                {
                    Id = u.User.Id,
                    UserName = u.User.UserName,
                    CanRead = u.CanRead,
                    CanWrite = u.CanWrite,
                    CanChangeAccess = u.CanChangeAccess,
                    IsOwner = u.IsOwner
                })
            };

            return Ok(result);
        }

        // GET: api/Groups
        [HttpGet]
        public IEnumerable<Group> GetGroups()
        {
            var user = GetCurrentUser();
            //todo: check if admin and return all
            return _context.Groups
                .AsNoTracking()
                .Include(g => g.Users)
                .Where(g => g.Users.Select(u => u.UserId).Contains(user.Id));
        }

        // POST: api/Groups
        [HttpPost]
        [ProducesResponseType(201, Type = typeof(Group))]
        [ProducesResponseType(400)]
        [ProducesResponseType(403)]
        public async Task<IActionResult> PostGroup([FromBody] Group group)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var user = GetCurrentUser();

            if (!_accessService.CheckWriteAccess(group, user))
            {
                return Forbid();
            }

            _context.Groups.Add(group);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetGroup", new { id = group.Id }, group);
        }

        // PUT: api/Groups/5
        [HttpPut("{id}")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(403)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> PutGroup([FromRoute] int id, [FromBody] Group group)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != group.Id)
            {
                return BadRequest();
            }

            var dbEntry = await _context.Groups.FindAsync(id);

            if (dbEntry == null)
            {
                return NotFound();
            }

            var user = GetCurrentUser();

            if (!_accessService.CheckWriteAccess(group, user))
            {
                return Forbid();
            }

            dbEntry.Name = group.Name;
            dbEntry.ParentId = group.ParentId;
            await _context.SaveChangesAsync();
            return Ok();
        }

        private ApplicationUser GetCurrentUser()
        {
            var id = User.Claims.SingleOrDefault(c => c.Type == "sub");
            var user = _context.Users.Find(id.Value);
            return user;
        }

        private bool GroupExists(int id)
        {
            return _context.Groups.Any(e => e.Id == id);
        }

        private bool UserExists(string id)
        {
            return _context.Users.Any(u => u.Id == id);
        }
    }
}
