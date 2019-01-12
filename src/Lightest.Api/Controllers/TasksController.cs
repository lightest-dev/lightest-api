using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Lightest.AccessService.Interfaces;
using Lightest.Api.ResponseModels;
using Lightest.Data;
using Lightest.Data.Models;
using Lightest.Data.Models.TaskModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Lightest.Api.Controllers
{
    [Produces("application/json")]
    [Route("[controller]")]
    [Authorize]
    public class TasksController : BaseUserController
    {
        private readonly IAccessService<TaskDefinition> _accessService;

        public TasksController(
            RelationalDbContext context,
            IAccessService<TaskDefinition> accessService,
            UserManager<ApplicationUser> userManager) : base(context, userManager)
        {
            _accessService = accessService;
        }

        // GET: api/Tasks
        [HttpGet]
        [ProducesResponseType(typeof(TaskDefinition), 200)]
        [ProducesResponseType(403)]
        public async Task<IActionResult> GetTasks()
        {
            var user = await GetCurrentUser();
            var tasks = _context.Tasks
                .AsNoTracking()
                .Where(t => t.Users.Select(u => u.UserId).Contains(user.Id));
            return Ok(tasks);
        }

        // GET: api/Tasks/5
        [HttpGet("{id}")]
        [ProducesResponseType(200, Type = typeof(CompleteTask))]
        [ProducesResponseType(400)]
        [ProducesResponseType(403)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> GetTask([FromRoute] int id)
        {
            var task = await _context.Tasks
                .AsNoTracking()
                .Include(t => t.Tests)
                .Include(t => t.Languages)
                .ThenInclude(l => l.Language)
                .Include(t => t.Category)
                .Include(t => t.Checker)
                .SingleOrDefaultAsync(t => t.Id == id);

            var user = await GetCurrentUser();

            if (task == null)
            {
                return NotFound();
            }

            if (!_accessService.CheckReadAccess(task, user))
            {
                return Forbid();
            }

            var result = new CompleteTask
            {
                Id = task.Id,
                Name = task.Name,
                Points = task.Points,
                Public = task.Public,
                Examples = task.Examples,
                Description = task.Description,
                Category = task.Category,
                Checker = new BasicNameViewModel
                {
                    Id = task.Checker.Id,
                    Name = task.Checker.Name
                },
                Tests = task.Tests,
                Languages = task.Languages.Select(t => new BasicLanguage
                {
                    Id = t.LanguageId,
                    Name = t.Language.Name,
                    MemoryLimit = t.MemoryLimit,
                    TimeLimit = t.TimeLimit
                })
            };

            if (!_accessService.CheckWriteAccess(task, user))
            {
                result.Tests = null;
                result.Checker = null;
            }

            return Ok(result);
        }

        [HttpGet("{id}/users")]
        [ProducesResponseType(200, Type = typeof(IEnumerable<AccessRightsUser>))]
        [ProducesResponseType(400)]
        [ProducesResponseType(403)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> GetUsers([FromRoute] int id)
        {
            var task = await _context.Tasks
                .AsNoTracking()
                .Include(t => t.Users)
                .ThenInclude(u => u.User)
                .SingleOrDefaultAsync(t => t.Id == id);

            if (task == null)
            {
                return NotFound();
            }

            if (!_accessService.CheckWriteAccess(task, await GetCurrentUser()))
            {
                return Forbid();
            }

            return Ok(task.Users.Select(u => new AccessRightsUser
            {
                Id = u.UserId,
                UserName = u.User.UserName,
                CanRead = u.CanRead,
                CanWrite = u.CanWrite,
                CanChangeAccess = u.CanChangeAccess,
                IsOwner = u.IsOwner
            }));
        }

        // POST: api/Tasks
        [HttpPost]
        [ProducesResponseType(typeof(TaskDefinition), 201)]
        [ProducesResponseType(400)]
        [ProducesResponseType(403)]
        public async Task<IActionResult> PostTask([FromBody] TaskDefinition task)
        {
            var user = await GetCurrentUser();

            if (user == null)
            {
                return BadRequest("id");
            }

            if (!_accessService.CheckWriteAccess(task, user))
            {
                return Forbid();
            }

            if (task.Public)
            {
                var category = await _context.Categories
                    .SingleOrDefaultAsync(c => c.Id == task.CategoryId);
                if (!category.Public)
                {
                    return BadRequest(nameof(task.Public));
                }
            }

            task.Users = new List<UserTask>
            {
                new UserTask { UserId = user.Id, CanRead = true, CanWrite = true, CanChangeAccess = true, IsOwner = true }
            };

            _context.Tasks.Add(task);

            await _context.SaveChangesAsync();

            return CreatedAtAction("GetTask", new { id = task.Id }, task);
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

            if (!_accessService.CheckWriteAccess(task, await GetCurrentUser()))
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
                    if (existingUser.IsOwner)
                    {
                        continue;
                    }
                    //todo: add rights check
                    existingUser.Deadline = user.Deadline;
                    existingUser.CanRead = user.CanRead;
                    existingUser.CanWrite = user.CanWrite;
                }
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

            if (!_accessService.CheckWriteAccess(task, await GetCurrentUser()))
            {
                return Forbid();
            }

            foreach (var l in languages)
            {
                l.TaskId = id;
            }

            task.Languages = languages;

            await _context.SaveChangesAsync();

            return Ok();
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

            if (!_accessService.CheckWriteAccess(task, await GetCurrentUser()))
            {
                return Forbid();
            }

            foreach (var test in tests)
            {
                test.TaskId = id;
            }

            task.Tests = tests;

            await _context.SaveChangesAsync();

            return Ok();
        }

        // PUT: api/Tasks/5
        [HttpPut("{id}")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(403)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> PutTask([FromRoute] int id, [FromBody] TaskDefinition task)
        {
            if (id != task.Id)
            {
                return BadRequest();
            }

            var dbEntry = _context.Tasks.Find(id);

            if (dbEntry == null)
            {
                return NotFound();
            }

            if (!_accessService.CheckWriteAccess(task, await GetCurrentUser()))
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

        // DELETE: api/Tasks/5
        [HttpDelete("{id}")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(403)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> DeleteTask([FromRoute] int id)
        {
            var task = await _context.Tasks.FindAsync(id);
            if (task == null)
            {
                return NotFound();
            }

            if (!_accessService.CheckWriteAccess(task, await GetCurrentUser()))
            {
                return Forbid();
            }

            _context.Tasks.Remove(task);
            await _context.SaveChangesAsync();

            return Ok(task);
        }
    }
}
