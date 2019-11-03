using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Lightest.AccessService.Interfaces;
using Lightest.Api.ResponseModels;
using Lightest.Api.ResponseModels.Checker;
using Lightest.Api.ResponseModels.Language;
using Lightest.Api.ResponseModels.TaskViews;
using Lightest.Data;
using Lightest.Data.Models;
using Lightest.Data.Models.TaskModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Sieve.Models;
using Sieve.Services;

namespace Lightest.Api.Controllers
{
    [Produces("application/json")]
    [Route("[controller]")]
    public class TasksController : BaseUserController
    {
        private readonly IAccessService<TaskDefinition> _accessService;
        private readonly ISieveProcessor _sieveProcessor;

        public TasksController(
            RelationalDbContext context,
            IAccessService<TaskDefinition> accessService,
            UserManager<ApplicationUser> userManager,
            ISieveProcessor sieveProcessor) : base(context, userManager)
        {
            _accessService = accessService;
            _sieveProcessor = sieveProcessor;
        }

        // GET: api/Tasks
        [HttpGet]
        [ProducesResponseType(typeof(TaskDefinition), 200)]
        [ProducesResponseType(403)]
        public async Task<IActionResult> GetTasks([FromQuery]SieveModel sieveModel)
        {
            var user = await GetCurrentUser();

            if (!_accessService.HasAdminAccess(user))
            {
                return Forbid();
            }

            var tasks = _context.Tasks.AsNoTracking();
            tasks = _sieveProcessor.Apply(sieveModel, tasks);

            return Ok(tasks);
        }

        [HttpGet("tasks")]
        [ProducesResponseType(200, Type = typeof(IEnumerable<UserTaskView>))]
        public IActionResult GetAssignedTasks()
        {
            var id = User.Claims.SingleOrDefault(c => c.Type == "sub").Value;
            var assignedTasks = _context.UserTasks.AsNoTracking()
                .Include(ut => ut.Task)
                .Where(ut => ut.UserId == id);

            return Ok(assignedTasks.Select(t => new UserTaskView
            {
                Id = t.TaskId,
                Name = t.Task.Name,
                Deadline = t.Deadline,
                Completed = t.Completed,
                HighScore = t.HighScore
            }));
        }

        // GET: api/Tasks/5
        [HttpGet("{id}")]
        [ProducesResponseType(200, Type = typeof(CompleteTaskView))]
        [ProducesResponseType(400)]
        [ProducesResponseType(403)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> GetTask([FromRoute] Guid id)
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

            if (!_accessService.HasReadAccess(task, user))
            {
                return Forbid();
            }

            var result = new CompleteTaskView
            {
                Id = task.Id,
                Name = task.Name,
                Points = task.Points,
                Public = task.Public,
                Examples = task.Examples,
                Description = task.Description,
                Category = task.Category,
                Checker = new BasicCheckerView
                {
                    Id = task.Checker.Id,
                    Name = task.Checker.Name,
                    Compiled = task.Checker.Compiled
                },
                Tests = task.Tests,
                Languages = task.Languages.Select(t => new BasicLanguageView
                {
                    Id = t.LanguageId,
                    Name = t.Language.Name,
                    MemoryLimit = t.MemoryLimit,
                    TimeLimit = t.TimeLimit
                })
            };

            if (!_accessService.HasWriteAccess(task, user))
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
        public async Task<IActionResult> GetUsers([FromRoute] Guid id)
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

            if (!_accessService.HasWriteAccess(task, await GetCurrentUser()))
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

            if (!_accessService.HasWriteAccess(task, user))
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

            task.Users = new List<Data.Models.UserTask>
            {
                new Data.Models.UserTask { UserId = user.Id, CanRead = true, CanWrite = true, CanChangeAccess = true, IsOwner = true }
            };

            _context.Tasks.Add(task);

            await _context.SaveChangesAsync();

            return CreatedAtAction("GetTask", new { id = task.Id }, task);
        }

        [HttpPost("{id}/users")]
        [ProducesResponseType(200)]
        [ProducesResponseType(403)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> AddUsers([FromRoute] Guid id, [FromBody] Data.Models.UserTask[] users)
        {
            var task = await _context.Tasks
               .Include(t => t.Users)
               .SingleOrDefaultAsync(t => t.Id == id);

            if (task == null)
            {
                return NotFound();
            }

            if (!_accessService.HasWriteAccess(task, await GetCurrentUser()))
            {
                return Forbid();
            }

            foreach (var user in users)
            {
                var existingUser = task.Users.SingleOrDefault(u => u.UserId == user.UserId);
                if (existingUser == null)
                {
                    user.IsOwner = false;
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
                    existingUser.CanChangeAccess = user.CanChangeAccess;
                }
            }
            await _context.SaveChangesAsync();
            return Ok();
        }

        [HttpPost("{id}/languages")]
        [ProducesResponseType(200)]
        [ProducesResponseType(403)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> SetLanguages([FromRoute] Guid id, [FromBody] TaskLanguage[] languages)
        {
            var task = await _context.Tasks
                .Include(t => t.Languages)
                .SingleOrDefaultAsync(t => t.Id == id);

            if (task == null)
            {
                return NotFound();
            }

            if (!_accessService.HasWriteAccess(task, await GetCurrentUser()))
            {
                return Forbid();
            }

            task.Languages.Clear();
            foreach (var language in languages)
            {
                language.TaskId = id;
                task.Languages.Add(language);
            }

            await _context.SaveChangesAsync();

            return Ok();
        }

        [HttpPost("{id}/tests")]
        [ProducesResponseType(200)]
        [ProducesResponseType(403)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> SetTests([FromRoute] Guid id, [FromBody] Test[] tests)
        {
            var task = await _context.Tasks
                .Include(t => t.Tests)
                .SingleOrDefaultAsync(t => t.Id == id);

            if (task == null)
            {
                return NotFound();
            }

            if (!_accessService.HasWriteAccess(task, await GetCurrentUser()))
            {
                return Forbid();
            }

            task.Tests.Clear();

            foreach (var test in tests)
            {
                test.TaskId = id;
                task.Tests.Add(test);
                _context.Tests.Add(test);
            }

            await _context.SaveChangesAsync();

            return Ok();
        }

        // PUT: api/Tasks/5
        [HttpPut("{id}")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(403)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> PutTask([FromRoute] Guid id, [FromBody] TaskDefinition task)
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

            if (!_accessService.HasWriteAccess(task, await GetCurrentUser()))
            {
                return Forbid();
            }

            dbEntry.CategoryId = task.CategoryId;
            dbEntry.Examples = task.Examples;
            dbEntry.Description = task.Description;
            dbEntry.Points = task.Points;
            dbEntry.Public = task.Public;
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
        public async Task<IActionResult> DeleteTask([FromRoute] Guid id)
        {
            var task = await _context.Tasks.FindAsync(id);
            if (task == null)
            {
                return NotFound();
            }

            if (!_accessService.HasWriteAccess(task, await GetCurrentUser()))
            {
                return Forbid();
            }

            _context.Tasks.Remove(task);
            await _context.SaveChangesAsync();

            return Ok(task);
        }
    }
}
