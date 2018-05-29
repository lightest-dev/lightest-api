using Lightest.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Lightest.Api.Controllers
{
    [Produces("application/json")]
    [Route("api/[controller]")]
    public class TasksController : Controller
    {
        private readonly RelationalDbContext _context;

        public TasksController(RelationalDbContext context)
        {
            _context = context;
        }

        // GET: api/Tasks
        [HttpGet]
        [ProducesResponseType(typeof(Task),200)]
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
                .SingleOrDefaultAsync(t =>t.Id == id);

            if (task == null)
            {
                return NotFound();
            }

            if (!CheckReadAccess(task))
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
                Tests = task.Tests.Select(t => new { t.Id, t.Input, t.Output }),
                Languages = task.Languages.Select(l => new { Id = l.LanguageId, l.Language.Name,
                    l.MemoryLimitation, l.TimeLimitation })
            });
        }

        [HttpGet("Users/{id}")]
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

            if (!CheckReadAccess(task))
            {
                return Forbid();
            }

            return Ok(task.Users.Select(u => new { u.UserId, u.User.UserName }));
        }

        // PUT: api/Tasks/5
        [HttpPut("{id}")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(403)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> PutTask([FromRoute] int id, [FromBody] Data.Models.Task task)
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

            if (!CheckWriteAccess(dbEntry))
            {
                return Forbid();
            }

            dbEntry.CategoryId = task.CategoryId;
            dbEntry.Examples = task.Examples;
            dbEntry.Points = task.Points;
            dbEntry.Public = dbEntry.Public;

            await _context.SaveChangesAsync();
            return Ok();
        }

        // POST: api/Tasks
        [HttpPost]
        [ProducesResponseType(typeof(Task),201)]
        [ProducesResponseType(400)]
        [ProducesResponseType(403)]
        public async Task<IActionResult> PostTask([FromBody] Data.Models.Task task)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _context.Tasks.Add(task);

            if (!CheckWriteAccess(task))
            {
                return Forbid();
            }

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

            if (!CheckWriteAccess(task))
            {
                return Forbid();
            }

            _context.Tasks.Remove(task);
            await _context.SaveChangesAsync();

            return Ok(task);
        }

        private bool TaskExists(int id)
        {
            return _context.Tasks.Any(e => e.Id == id);
        }

        private bool CheckListAccess()
        {
            return true;
        }

        private bool CheckReadAccess(Data.Models.Task task)
        {
            return true;
        }

        private bool CheckWriteAccess(Data.Models.Task task)
        {
            return true;
        }
    }
}