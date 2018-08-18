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
    [AllowAnonymous]
    public class CategoriesController : Controller
    {
        private readonly IAccessService<Category> _accessService;
        private readonly RelationalDbContext _context;

        public CategoriesController(RelationalDbContext context, IAccessService<Category> accessService)
        {
            _context = context;
            _accessService = accessService;
        }

        [HttpPost("{id}/access")]
        [ProducesResponseType(200)]
        [ProducesResponseType(403)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> ChangeAccess([FromRoute] int id, [FromBody]AccessRightsViewModel model)
        {
            var category = await _context.Categories
                .Include(c => c.Users)
                .FirstOrDefaultAsync(c => c.Id == id);

            if (category == null)
            {
                return NotFound();
            }

            var currentUser = GetCurrentUser();

            if (!_accessService.CheckAdminAccess(category, currentUser))
            {
                return Forbid();
            }

            if (!_context.Users.Any(u => u.Id == model.UserId))
            {
                return BadRequest();
            }

            var user = category.Users.SingleOrDefault(u => u.UserId == model.UserId);
            if (user == null)
            {
                var categoryUser = new CategoryUser { CategoryId = category.Id, UserId = model.UserId };
                model.CopyTo(categoryUser);
                category.Users.Add(categoryUser);
            }
            else
            {
                model.CopyTo(user);
            }
            await _context.SaveChangesAsync();
            return Ok();
        }

        // DELETE: api/Categories/5
        [HttpDelete("{id}")]
        [ProducesResponseType(200, Type = typeof(Category))]
        [ProducesResponseType(400)]
        [ProducesResponseType(403)]
        [ProducesResponseType(404)]
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

            var currentUser = GetCurrentUser();

            if (!_accessService.CheckWriteAccess(category, currentUser))
            {
                return Forbid();
            }

            _context.Categories.Remove(category);
            await _context.SaveChangesAsync();

            return Ok(category);
        }

        // GET: api/Categories
        [HttpGet]
        [ProducesResponseType(200, Type = typeof(Category))]
        public IActionResult GetCategories()
        {
            return Ok(_context.Categories);
        }

        // GET: api/Categories/5
        [HttpGet("{id}")]
        [ProducesResponseType(200, Type = typeof(CompleteCategoryViewModel))]
        [ProducesResponseType(400)]
        [ProducesResponseType(403)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> GetCategory([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var category = await _context.Categories
                .AsNoTracking()
                .Include(c => c.SubCategories)
                .Include(c => c.Users)
                .ThenInclude(u => u.User)
                .Include(c => c.Tasks)
                .FirstOrDefaultAsync(c => c.Id == id);

            var currentUser = GetCurrentUser();

            if (category == null)
            {
                return NotFound();
            }

            if (!_accessService.CheckReadAccess(category, currentUser))
            {
                return Forbid();
            }

            var result = new CompleteCategoryViewModel
            {
                Id = category.Id,
                Name = category.Name,
                Parent = category.Parent,
                SubCategories = category.SubCategories,
                Users = category.Users.Select(user => new AccessRightsUserViewModel
                {
                    Id = user.User.Id,
                    UserName = user.User.UserName,
                    CanRead = user.CanRead,
                    CanWrite = user.CanWrite,
                    CanChangeAccess = user.CanChangeAccess,
                    IsOwner = user.IsOwner
                }),
                Tasks = category.Tasks.Select(t => new BasicTaskViewModel
                {
                    Id = t.Id,
                    Name = t.Name
                })
            };

            return Ok(result);
        }

        // POST: api/Categories
        [HttpPost]
        [ProducesResponseType(201, Type = typeof(CompleteCategoryViewModel))]
        [ProducesResponseType(400)]
        [ProducesResponseType(403)]
        public async Task<IActionResult> PostCategory([FromBody] Category category)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var currentUser = GetCurrentUser();

            if (!_accessService.CheckWriteAccess(category, currentUser))
            {
                return Forbid();
            }

            _context.Categories.Add(category);
            category.Users = new List<CategoryUser>();
            var categoryUser = new CategoryUser { UserId = currentUser.Id, CategoryId = category.Id };
            categoryUser.SetFullRights();
            category.Users.Add(categoryUser);
            await _context.SaveChangesAsync();
            return CreatedAtAction("GetCategory", new { id = category.Id }, category);
        }

        // PUT: api/Categories/5
        [HttpPut("{id}")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(403)]
        [ProducesResponseType(404)]
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

            var currentUser = GetCurrentUser();

            if (!_accessService.CheckWriteAccess(dbEntry, currentUser))
            {
                return Forbid();
            }

            dbEntry.Name = category.Name;
            dbEntry.ParentId = category.ParentId;
            await _context.SaveChangesAsync();
            return Ok();
        }

        private bool CategoryExists(int id)
        {
            return _context.Categories.Any(e => e.Id == id);
        }

        private ApplicationUser GetCurrentUser()
        {
            var id = User.Claims.SingleOrDefault(c => c.Type == "sub");
            var user = _context.Users.Find(id.Value);
            return user;
        }
    }
}
