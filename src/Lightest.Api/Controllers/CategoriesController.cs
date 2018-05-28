using Lightest.Data;
using Lightest.Data.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

namespace Lightest.Api.Controllers
{
    [Produces("application/json")]
    [Route("api/[controller]")]
    public class CategoriesController : Controller
    {
        private readonly RelationalDbContext _context;

        public CategoriesController(RelationalDbContext context)
        {
            _context = context;
        }

        // GET: api/Categories
        [HttpGet]
        public IActionResult GetCategories()
        {
            return Ok(_context.Categories);
        }

        // GET: api/Categories/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetCategory([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var category = await _context.Categories
                .Include(c => c.SubCategories)
                .Include(c => c.Users)
                .ThenInclude(u => u.User)
                .Include(c => c.Tasks)
                .FirstOrDefaultAsync(c => c.Id == id);
            if (!CheckReadAccess(category))
            {
                return Forbid();
            }

            if (category == null)
            {
                return NotFound();
            }

            return Ok(new
            {
                category.Id,
                category.Name,
                category.Parent,
                category.SubCategories,
                Users = category.Users.Select(u => new { u.User.Id, u.User.UserName, u.UserRights }),
                Tasks = category.Tasks.Select(t => new { t.Id, t.Name })
            });
        }

        // PUT: api/Categories/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutCategory([FromRoute] int id, [FromBody] Category category)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != category.Id)
            {
                return BadRequest();
            }

            var dbEntry = await _context.Categories.FindAsync(id);

            if (dbEntry == null)
            {
                return NotFound();
            }

            if (!CheckWriteAccess(dbEntry))
            {
                return Forbid();
            }
            dbEntry.Name = category.Name;
            dbEntry.ParentId = category.ParentId;
            await _context.SaveChangesAsync();
            return Ok();
        }

        // POST: api/Categories
        [HttpPost]
        public async Task<IActionResult> PostCategory([FromBody] Category category)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (!CheckWriteAccess(category))
            {
                return Forbid();
            }

            _context.Categories.Add(category);
            await _context.SaveChangesAsync();
            category.Users.Add(new CategoryUser { UserId = User.Identity.Name, CategoryId = category.Id, UserRights = AccessRights.Owner });
            await _context.SaveChangesAsync();
            return CreatedAtAction("GetCategory", new { id = category.Id }, category);
        }

        // DELETE: api/Categories/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCategory([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var category = await _context.Categories.FindAsync(id);
            if (category == null)
            {
                return NotFound();
            }
            if (!CheckWriteAccess(category))
            {
                return StatusCode(403);
            }
            _context.Categories.Remove(category);
            await _context.SaveChangesAsync();

            return Ok(category);
        }

        [HttpPost("ChangeAccess/{id}")]
        public async Task<IActionResult> ChangeAccess([FromRoute] int id, [FromBody]string userId, [FromBody]int accessRights)
        {
            if (!CategoryExists(id))
            {
                return NotFound();
            }
            var access = (AccessRights)accessRights;
            var category = await _context.Categories
                .Include(c => c.Users)
                .FirstOrDefaultAsync(c => c.Id == id);
            if (!CheckAdminAccess(category))
            {
                return Forbid();
            }
            if (category == null || !_context.Users.Any(u => u.Id == userId))
            {
                return BadRequest();
            }

            var user = category.Users.SingleOrDefault(u => u.UserId == userId);
            if (user == null)
            {
                category.Users.Add(new CategoryUser { CategoryId = category.Id, UserId = userId, UserRights = access });
            }
            else
            {
                user.UserRights = access;
            }
            await _context.SaveChangesAsync();
            return Ok();
        }

        private bool CategoryExists(int id)
        {
            return _context.Categories.Any(e => e.Id == id);
        }

        private bool CheckWriteAccess(Category category)
        {
            return true;
        }

        private bool CheckReadAccess(Category category)
        {
            return true;
        }

        private bool CheckAdminAccess(Category category)
        {
            return true;
        }
    }
}