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
    public class CategoriesController : BaseUserController
    {
        private readonly IAccessService<Category> _accessService;

        public CategoriesController(
            RelationalDbContext context,
            IAccessService<Category> accessService,
            UserManager<ApplicationUser> userManager) : base(context, userManager)
        {
            _accessService = accessService;
        }

        // GET: api/Categories
        [HttpGet]
        [ProducesResponseType(200, Type = typeof(Category))]
        public async Task<IActionResult> GetCategories()
        {
            var user = await GetCurrentUser();
            var categories = _context.Categories
                .AsNoTracking()
                .Include(c => c.Users)
                .Where(c => (c.Public || c.Users.Select(u => u.UserId).Contains(user.Id))
                && c.ParentId == null);
            return Ok(categories);
        }

        [HttpGet("{id}/children")]
        [ProducesResponseType(200, Type = typeof(CategoryChildrenViewModel))]
        public async Task<IActionResult> GetChildren(int id)
        {
            var user = await GetCurrentUser();
            var categories = _context.Categories
                .AsNoTracking()
                .Include(c => c.Users)
                .Where(c => (c.Public || c.Users.Select(u => u.UserId).Contains(user.Id))
                    && c.ParentId == id);
            var tasks = _context.Tasks
                .Include(t => t.Users)
                .Where(t => t.CategoryId == id &&
                    (t.Public || t.Users.Select(u => u.UserId).Contains(user.Id)));
            var result = new CategoryChildrenViewModel
            {
                SubCategories = categories,
                Tasks = tasks.Select(t => new BasicNameViewModel
                {
                    Name = t.Name,
                    Id = t.Id
                })
            };
            return Ok(result);
        }

        // GET: api/Categories/5
        [HttpGet("{id}")]
        [ProducesResponseType(200, Type = typeof(CompleteCategory))]
        [ProducesResponseType(400)]
        [ProducesResponseType(403)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> GetCategory([FromRoute] int id)
        {
            var category = await _context.Categories
                .AsNoTracking()
                .Include(c => c.SubCategories)
                .Include(c => c.Users)
                .ThenInclude(u => u.User)
                .Include(c => c.Tasks)
                .FirstOrDefaultAsync(c => c.Id == id);

            var currentUser = await GetCurrentUser();

            if (category == null)
            {
                return NotFound();
            }

            if (!_accessService.CheckWriteAccess(category, currentUser))
            {
                return Forbid();
            }

            var result = new CompleteCategory
            {
                Id = category.Id,
                Name = category.Name,
                Parent = category.Parent,
                SubCategories = category.SubCategories,
                Users = category.Users.Select(user => new AccessRightsUser
                {
                    Id = user.User.Id,
                    UserName = user.User.UserName,
                    CanRead = user.CanRead,
                    CanWrite = user.CanWrite,
                    CanChangeAccess = user.CanChangeAccess,
                    IsOwner = user.IsOwner
                }),
                Tasks = category.Tasks.Select(t => new BasicNameViewModel
                {
                    Id = t.Id,
                    Name = t.Name
                })
            };

            return Ok(result);
        }

        // POST: api/Categories
        [HttpPost]
        [ProducesResponseType(201, Type = typeof(CompleteCategory))]
        [ProducesResponseType(400)]
        [ProducesResponseType(403)]
        public async Task<IActionResult> PostCategory([FromBody] Category category)
        {
            var currentUser = await GetCurrentUser();

            if (!_accessService.CheckWriteAccess(category, currentUser))
            {
                return Forbid();
            }

            if (category.Public && category.ParentId != null)
            {
                var parent = await _context.Categories
                    .AsNoTracking()
                    .SingleOrDefaultAsync(t => t.Id == category.ParentId);
                if (parent == null || !parent.Public)
                {
                    return BadRequest(nameof(category.ParentId));
                }
            }

            _context.Categories.Add(category);
            category.Users = new List<CategoryUser>();
            var categoryUser = new CategoryUser { UserId = currentUser.Id, CategoryId = category.Id };
            categoryUser.SetFullRights();
            category.Users.Add(categoryUser);
            await _context.SaveChangesAsync();
            return CreatedAtAction("GetCategory", new { id = category.Id }, category);
        }

        [HttpPost("{id}/add-users")]
        [ProducesResponseType(200)]
        [ProducesResponseType(403)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> AddUsers([FromRoute] int id, [FromBody]IEnumerable<AccessRights> users)
        {
            var category = await _context.Categories
                .Include(c => c.Users)
                .FirstOrDefaultAsync(c => c.Id == id);

            if (category == null)
            {
                return NotFound();
            }

            var currentUser = await GetCurrentUser();

            if (!_accessService.CheckAdminAccess(category, currentUser))
            {
                return Forbid();
            }

            foreach (var user in users)
            {
                if (!_context.Users.Any(u => u.Id == user.UserId))
                {
                    return BadRequest();
                }

                var categoryUser = category.Users.SingleOrDefault(u => u.UserId == user.UserId);
                if (categoryUser == null)
                {
                    categoryUser = new CategoryUser { CategoryId = category.Id, UserId = user.UserId };
                    user.CopyTo(categoryUser);
                    category.Users.Add(categoryUser);
                }
                else
                {
                    user.CopyTo(categoryUser);
                }
            }

            await _context.SaveChangesAsync();
            return Ok();
        }

        // PUT: api/Categories/5
        [HttpPut("{id}")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(403)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> PutCategory([FromRoute] int id, [FromBody] Category category)
        {
            if (id != category.Id)
            {
                return BadRequest();
            }

            var dbEntry = await _context.Categories.FindAsync(id);

            if (dbEntry == null)
            {
                return NotFound();
            }

            var currentUser = await GetCurrentUser();

            if (!_accessService.CheckWriteAccess(dbEntry, currentUser))
            {
                return Forbid();
            }

            dbEntry.Name = category.Name;
            dbEntry.ParentId = category.ParentId;
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
            var category = await _context.Categories.FindAsync(id);
            if (category == null)
            {
                return NotFound();
            }

            var currentUser = await GetCurrentUser();

            if (!_accessService.CheckWriteAccess(category, currentUser))
            {
                return Forbid();
            }

            _context.Categories.Remove(category);
            await _context.SaveChangesAsync();

            return Ok(category);
        }
    }
}
