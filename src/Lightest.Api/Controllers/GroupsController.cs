using Lightest.Data;
using Lightest.Data.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Lightest.Api.Controllers
{
    [Produces("application/json")]
    [Route("api/[controller]")]
    public class GroupsController : Controller
    {
        private readonly RelationalDbContext _context;

        public GroupsController(RelationalDbContext context)
        {
            _context = context;
        }

        // GET: api/Groups
        [HttpGet]
        public IEnumerable<Group> GetGroups()
        {
            return _context.Groups;
        }

        // GET: api/Groups/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetGroup([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var group = await _context.Groups
                .Include(g => g.SubGroups)
                .Include(g => g.Users)
                .ThenInclude(u => u.User)
                .Where(g => g.Id == id)
                .SingleOrDefaultAsync();

            if (group == null)
            {
                return NotFound();
            }

            if (!CheckReadAccess(group))
            {
                return Forbid();
            }

            return Ok(new
            {
                group.Id,
                group.Name,
                group.ParentId,
                Users = group.Users.Select(u => new { u.UserId, u.User.UserName }),
                Subgroups = group.SubGroups.Select(g => new { g.Id, g.Name })
            });
        }

        // PUT: api/Groups/5
        [HttpPut("{id}")]
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

            if (!CheckWriteAccess(dbEntry))
            {
                return Forbid();
            }

            dbEntry.Name = group.Name;
            dbEntry.ParentId = group.ParentId;
            await _context.SaveChangesAsync();
            return Ok();
        }

        // POST: api/Groups
        [HttpPost]
        public async Task<IActionResult> PostGroup([FromBody] Group group)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (!CheckWriteAccess(group))
            {
                return Forbid();
            }

            _context.Groups.Add(group);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetGroup", new { id = group.Id }, group);
        }

        // DELETE: api/Groups/5
        [HttpDelete("{id}")]
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

            if (!CheckWriteAccess(group))
            {
                return Forbid();
            }

            _context.Groups.Remove(group);
            await _context.SaveChangesAsync();

            return Ok(group);
        }

        [HttpPost("{groupId}/AddUser/{userID}")]
        public async Task<IActionResult> AddUser([FromRoute] int groupId, [FromRoute]string userId)
        {
            var group = await _context.Groups.FindAsync(groupId);
            var user = await _context.Users.FindAsync(userId);
            if (user == null || group == null)
            {
                return NotFound();
            }
            if (!CheckWriteAccess(group))
            {
                return Forbid();
            }
            group.Users.Add(new UserGroup { GroupId = group.Id, UserId = user.Id });
            await _context.SaveChangesAsync();
            return Ok();
        }

        private bool GroupExists(int id)
        {
            return _context.Groups.Any(e => e.Id == id);
        }

        private bool CheckReadAccess(Group group)
        {
            return true;
        }

        private bool CheckWriteAccess(Group group)
        {
            return true;
        }
    }
}