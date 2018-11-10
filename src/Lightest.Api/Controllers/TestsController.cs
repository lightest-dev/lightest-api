using System.Linq;
using System.Threading.Tasks;
using Lightest.Api.Services.AccessServices;
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
    [Route("api/[controller]")]
    [Authorize]
    public class TestsController : Controller
    {
        private readonly IAccessService<TaskDefinition> _accessService;
        private readonly RelationalDbContext _context;

        public TestsController(RelationalDbContext context, IAccessService<TaskDefinition> accessService)
        {
            _context = context;
            _accessService = accessService;
        }

        // GET: api/Tests/5
        [HttpGet("{id}")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(403)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> GetTest([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var test = await _context.Tests
                .AsNoTracking()
                .Include(t => t.Task)
                .SingleOrDefaultAsync(t => t.Id == id);

            if (test == null)
            {
                return NotFound();
            }

            //user can only view test if he can edit it
            if (!_accessService.CheckWriteAccess(test.Task, await GetCurrentUser()))
            {
                return Forbid();
            }

            return Ok(test);
        }

        // POST: api/Tests
        [HttpPost]
        [ProducesResponseType(201, Type = typeof(Test))]
        [ProducesResponseType(400)]
        [ProducesResponseType(403)]
        public async Task<IActionResult> PostTest([FromBody] Test test)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var task = await _context.Tasks.FindAsync(test.Id);

            if (task == null)
            {
                return BadRequest();
            }

            if (!_accessService.CheckWriteAccess(task, await GetCurrentUser()))
            {
                return Forbid();
            }

            _context.Tests.Add(test);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetTest", new { id = test.Id }, test);
        }

        // PUT: api/Tests/5
        [HttpPut("{id}")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(403)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> PutTest([FromRoute] int id, [FromBody] Test test)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != test.Id)
            {
                return BadRequest();
            }

            var dbEntry = await _context.Tests
                .Include(t => t.Task)
                .SingleOrDefaultAsync(t => t.Id == id);

            if (dbEntry == null)
            {
                return NotFound();
            }

            if (!_accessService.CheckWriteAccess(dbEntry.Task, await GetCurrentUser()))
            {
                return Forbid();
            }

            dbEntry.Input = test.Input;
            dbEntry.Output = test.Output;

            await _context.SaveChangesAsync();
            return Ok();
        }

        // DELETE: api/Tests/5
        [HttpDelete("{id}")]
        [ProducesResponseType(200, Type = typeof(Test))]
        [ProducesResponseType(400)]
        [ProducesResponseType(403)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> DeleteTest([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var test = await _context.Tests
                            .Include(t => t.Task)
                            .SingleOrDefaultAsync(t => t.Id == id);

            if (test == null)
            {
                return NotFound();
            }

            if (!_accessService.CheckWriteAccess(test.Task, await GetCurrentUser()))
            {
                return Forbid();
            }

            _context.Tests.Remove(test);
            await _context.SaveChangesAsync();

            return Ok(test);
        }

        private async Task<ApplicationUser> GetCurrentUser()
        {
            var id = User.Claims.SingleOrDefault(c => c.Type == "sub");
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == id.Value);
            return user;
        }

        private bool TestExists(int id)
        {
            return _context.Tests.Any(e => e.Id == id);
        }
    }
}
