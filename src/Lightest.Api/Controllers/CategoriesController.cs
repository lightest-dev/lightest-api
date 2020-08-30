using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Lightest.AccessService.Interfaces;
using Lightest.Api.Extensions;
using Lightest.Api.Models;
using Lightest.Api.ResponseModels;
using Lightest.Api.ResponseModels.CategoryViews;
using Lightest.Data;
using Lightest.Data.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Sieve.Models;
using Sieve.Services;

namespace Lightest.Api.Controllers
{
    [Produces("application/json")]
    [Route("[controller]")]
    public class CategoriesController : BaseUserController
    {
        private readonly IAccessService<Category> _accessService;
        private readonly ISieveProcessor _sieveProcessor;
        private readonly IRoleHelper _roleHelper;

        public CategoriesController(
            RelationalDbContext context,
            UserManager<ApplicationUser> userManager,
            IAccessService<Category> accessService,
            IRoleHelper roleHelper,
            ISieveProcessor sieveProcessor) : base(context, userManager)
        {
            _accessService = accessService;
            _sieveProcessor = sieveProcessor;
            _roleHelper = roleHelper;
        }

        [HttpGet("all")]
        [ProducesResponseType(200, Type = typeof(IEnumerable<Category>))]
        [ProducesResponseType(403)]
        public async Task<IActionResult> GetCategories([FromQuery] SieveModel sieveModel)
        {
            var user = await GetCurrentUser();
            var categories = _context.Categories
                .AsNoTracking();

            if (!await _roleHelper.IsAdmin(user))
            {
                return Forbid();
            }

            categories = _sieveProcessor.Apply(sieveModel, categories);

            return Ok(categories);
        }

        [HttpGet]
        [ProducesResponseType(200, Type = typeof(IEnumerable<Category>))]
        public async Task<IActionResult> GetAvailableCategories([FromQuery] SieveModel sieveModel)
        {
            var user = await GetCurrentUser();
            var categories = _context.Categories
                .AsNoTracking().Include(c => c.Users)
                .Where(c => (c.Public || c.Users.Select(u => u.UserId)
                    .Contains(user.Id)) && c.ParentId == null && !c.Contest);

            categories = _sieveProcessor.Apply(sieveModel, categories);

            var result = categories.Select(c => new ListCategoryView
            {
                Id = c.Id,
                Name = c.Name,
                Public = c.Public,
                User = c.Users.FirstOrDefault(u => u.UserId == user.Id)
            }).ToList();

            foreach (var category in result)
            {
                if (category.User == null)
                {
                    category.CanRead = true;
                    continue;
                }

                category.CanWrite = category.User.CanWrite;
                category.CanRead = category.User.CanRead;
                category.CanChangeAccess = category.User.CanChangeAccess;
            }

            return Ok(result);
        }

        [HttpGet("{id}/children")]
        [ProducesResponseType(200, Type = typeof(CategoryChildrenView))]
        public async Task<IActionResult> GetChildren(Guid id)
        {
            var user = await GetCurrentUser();

            var parent = await _context.Categories
                .AsNoTracking()
                .SingleOrDefaultAsync(c => c.Id == id);

            if (parent == null)
            {
                return NotFound();
            }

            if (!(await _accessService.CanRead(parent.Id, user)))
            {
                return Forbid();
            }

            var categories = _context.Categories
                .AsNoTracking()
                .Include(c => c.Users)
                .Where(c => (c.Public || c.Users.Select(u => u.UserId).Contains(user.Id))
                    && c.ParentId == id);

            var tasks = _context.Tasks
                .Include(t => t.Users)
                .Where(t => t.CategoryId == id
                    && (t.Public || t.Users.Select(u => u.UserId).Contains(user.Id)));

            var result = new CategoryChildrenView
            {
                SubCategories = categories,
                Tasks = tasks.Select(t => new BasicNameView
                {
                    Name = t.Name,
                    Id = t.Id
                })
            };
            return Ok(result);
        }

        // GET: api/Categories/5
        [HttpGet("{id}")]
        [ProducesResponseType(200, Type = typeof(CompleteCategoryView))]
        [ProducesResponseType(400)]
        [ProducesResponseType(403)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> GetCategory([FromRoute] Guid id)
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

            // full info is disclosed here, so write access is required
            if (!await _accessService.CanEdit(category.Id, currentUser))
            {
                return Forbid();
            }

            var result = new CompleteCategoryView
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
                Tasks = category.Tasks.Select(t => new BasicNameView
                {
                    Id = t.Id,
                    Name = t.Name
                })
            };

            return Ok(result);
        }

        // POST: api/Categories
        [HttpPost]
        [ProducesResponseType(201, Type = typeof(CompleteCategoryView))]
        [ProducesResponseType(400)]
        [ProducesResponseType(403)]
        public async Task<IActionResult> PostCategory([FromBody] Category category)
        {
            var currentUser = await GetCurrentUser();

            if (!(await _accessService.CanAdd(category, currentUser)))
            {
                return Forbid();
            }

            if (category.Contest && category.ParentId != null)
            {
                return BadRequest(nameof(category.Contest));
            }

            var publicCheckResult = await CheckIfCategoryCanBePublic(category);

            if (publicCheckResult != null)
            {
                return publicCheckResult;
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
        public async Task<IActionResult> AddUsers([FromRoute] Guid id, [FromBody] IEnumerable<AccessRights> users)
        {
            var category = await _context.Categories
                .Include(c => c.Users)
                .FirstOrDefaultAsync(c => c.Id == id);

            if (category == null)
            {
                return NotFound();
            }

            var currentUser = await GetCurrentUser();

            if (!await _accessService.CanEdit(category.Id, currentUser))
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
                //todo: check if user has access to parent category (if parent category is set)
                // if user cannot read parent, he should not have access to child
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
        public async Task<IActionResult> PutCategory([FromRoute] Guid id, [FromBody] Category category)
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

            if (!(await _accessService.CanEdit(category.Id, currentUser)))
            {
                return Forbid();
            }

            var publicCheckResult = await CheckIfCategoryCanBePublic(category);

            if (publicCheckResult != null)
            {
                return publicCheckResult;
            }

            dbEntry.Name = category.Name;
            dbEntry.ParentId = category.ParentId;
            dbEntry.Public = category.Public;
            await _context.SaveChangesAsync();
            return Ok();
        }

        // DELETE: api/Categories/5
        [HttpDelete("{id}")]
        [ProducesResponseType(200, Type = typeof(Category))]
        [ProducesResponseType(400)]
        [ProducesResponseType(403)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> DeleteCategory([FromRoute] Guid id)
        {
            var category = await _context.Categories.FindAsync(id);
            if (category == null)
            {
                return NotFound();
            }

            var currentUser = await GetCurrentUser();

            if (!await _accessService.CanEdit(category.Id, currentUser))
            {
                return Forbid();
            }

            _context.Categories.Remove(category);
            await _context.SaveChangesAsync();

            return Ok(category);
        }

        private async Task<IActionResult> CheckIfCategoryCanBePublic(Category category)
        {
            if (category.Public && category.ParentId != null)
            {
                var parent = await _context.Categories
                    .AsNoTracking()
                    .SingleOrDefaultAsync(t => t.Id == category.ParentId);
                if (parent?.Public != true)
                {
                    return BadRequest(nameof(category.ParentId));
                }
            }

            return null;
        }
    }
}
