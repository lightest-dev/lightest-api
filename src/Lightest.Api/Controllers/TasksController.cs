using Lightest.Data;
using Lightest.Data.Models;
using Lightest.Data.Models.TaskModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Lightest.Api.Controllers
{
    [Produces("application/json")]
    [Route("api/[controller]")]
    [Authorize]
    public class TasksController : Controller
    {
        private readonly RelationalDbContext _context;

        public TasksController(RelationalDbContext context)
        {
            _context = context;
        }

        // GET: api/Tasks
        [HttpGet]
        [ProducesResponseType(typeof(TaskDefinition), 200)]
        [ProducesResponseType(403)]
        public IActionResult GetTasks()
        {
            if (!CheckListAccess())
            {
                return Forbid();
            }
            return Ok(_context.Tasks);
        }

        // GET: api/Tasks/5
        [HttpGet("{id}")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(403)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> GetTask([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var task = await _context.Tasks
                .Include(t => t.Tests)
                .Include(t => t.Languages)
                .ThenInclude(l => l.Language)
                .SingleOrDefaultAsync(t => t.Id == id);

            if (task == null)
            {
                return NotFound();
            }

            if (!task.CheckReadAccess(GetCurrentUser()))
            {
                return Forbid();
            }

            return Ok(new
            {
                task.Id,
                task.Name,
                task.Points,
                task.Public,
                task.Examples,
                task.Description,
                task.CategoryId,
                task.CheckerId,
                Tests = task.Tests.Select(t => new { t.Id, t.Input, t.Output }),
                Languages = task.Languages.Select(l => new
                {
                    Id = l.LanguageId,
                    l.Language.Name,
                    l.MemoryLimitation,
                    l.TimeLimitation
                })
            });
        }

        [HttpGet("{id}/users")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(403)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> GetUsers([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var task = await _context.Tasks
                .Include(t => t.Users)
                .ThenInclude(u => u.User)
                .SingleOrDefaultAsync(t => t.Id == id);

            if (task == null)
            {
                return NotFound();
            }

            if (!task.CheckReadAccess(GetCurrentUser()))
            {
                return Forbid();
            }

            return Ok(task.Users.Select(u => new { u.UserId, u.User.UserName, u.UserRights }));
        }

        // PUT: api/Tasks/5
        [HttpPut("{id}")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(403)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> PutTask([FromRoute] int id, [FromBody] TaskDefinition task)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != task.Id)
            {
                return BadRequest();
            }

            var dbEntry = _context.Tasks.Find(id);

            if (dbEntry == null)
            {
                return NotFound();
            }

            if (!task.CheckWriteAccess(GetCurrentUser()))
            {
                return Forbid();
            }

            dbEntry.CategoryId = task.CategoryId;
            dbEntry.Examples = task.Examples;
            dbEntry.Points = task.Points;
            dbEntry.Public = dbEntry.Public;
            dbEntry.CheckerId = task.CheckerId;

            await _context.SaveChangesAsync();
            return Ok();
        }

        // POST: api/Tasks
        [HttpPost]
        [ProducesResponseType(typeof(TaskDefinition), 201)]
        [ProducesResponseType(400)]
        [ProducesResponseType(403)]
        public async Task<IActionResult> PostTask([FromBody] TaskDefinition task)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var user = GetCurrentUser();

            if (user == null)
            {
                return BadRequest("id");
            }

            if (!task.CheckWriteAccess(user))
            {
                return Forbid();
            }

            task.Users = new List<UserTask>
            {
                new UserTask { UserId = user.Id, UserRights = AccessRights.Owner | AccessRights.Read | AccessRights.Write | AccessRights.AssignAdmin }
            };

            _context.Tasks.Add(task);

            await _context.SaveChangesAsync();

            return CreatedAtAction("GetTask", new { id = task.Id }, task);
        }

        // DELETE: api/Tasks/5
        [HttpDelete("{id}")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(403)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> DeleteTask([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var task = await _context.Tasks.FindAsync(id);
            if (task == null)
            {
                return NotFound();
            }

            if (!task.CheckWriteAccess(GetCurrentUser()))
            {
                return Forbid();
            }

            _context.Tasks.Remove(task);
            await _context.SaveChangesAsync();

            return Ok(task);
        }

        [HttpPost("{id}/tests")]
        [ProducesResponseType(200)]
        [ProducesResponseType(403)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> SetTests([FromRoute] int id, [FromBody] Test[] tests)
        {
            var task = await _context.Tasks
                .Include(t => t.Tests)
                .SingleOrDefaultAsync(t => t.Id == id);

            if (task == null)
            {
                return NotFound();
            }

            if (!task.CheckWriteAccess(GetCurrentUser()))
            {
                return Forbid();
            }

            task.Tests.Clear();

            foreach (var test in tests)
            {
                test.TaskId = id;
                task.Tests.Add(test);
            }

            await _context.SaveChangesAsync();

            return Ok();
        }

        [HttpPost("{id}/languages")]
        [ProducesResponseType(200)]
        [ProducesResponseType(403)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> SetLanguages([FromRoute] int id, [FromBody] TaskLanguage[] languages)
        {
            var task = await _context.Tasks
                .Include(t => t.Languages)
                .SingleOrDefaultAsync(t => t.Id == id);

            if (task == null)
            {
                return NotFound();
            }

            if (!task.CheckWriteAccess(GetCurrentUser()))
            {
                return Forbid();
            }

            task.Languages.Clear();

            foreach (var l in languages)
            {
                l.TaskId = id;
                task.Languages.Add(l);
            }

            await _context.SaveChangesAsync();

            return Ok();
        }

        [HttpPost("{id}/users")]
        [ProducesResponseType(200)]
        [ProducesResponseType(403)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> AddUsers([FromRoute] int id, [FromBody] UserTask[] users)
        {
            var task = await _context.Tasks
               .Include(t => t.Users)
               .SingleOrDefaultAsync(t => t.Id == id);

            if (task == null)
            {
                return NotFound();
            }

            if (!task.CheckWriteAccess(GetCurrentUser()))
            {
                return Forbid();
            }

            foreach (var user in users)
            {
                var existingUser = task.Users.SingleOrDefault(u => u.UserId == user.UserId);
                if (existingUser == null)
                {
                    user.TaskId = id;
                    task.Users.Add(user);
                }
                else
                {
                    if (existingUser.UserRights.HasFlag(AccessRights.Owner))
                    {
                        continue;
                    }
                    //todo: add rights check
                    existingUser.Deadline = user.Deadline;
                    existingUser.UserRights = user.UserRights;
                }
            }
            await _context.SaveChangesAsync();
            return Ok();
        }

        private bool TaskExists(int id)
        {
            return _context.Tasks.Any(e => e.Id == id);
        }

        private bool CheckListAccess()
        {
            return true;
        }

        private ApplicationUser GetCurrentUser()
        {
            var id = User.Claims.SingleOrDefault(c => c.Type == "sub");
            var user = _context.Users.Find(id.Value);
            return user;
        }
    }
}