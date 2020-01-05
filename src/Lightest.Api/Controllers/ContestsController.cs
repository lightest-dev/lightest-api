using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Lightest.Api.RequestModels.ContestRequests;
using Lightest.Api.ResponseModels.ContestViews;
using Lightest.Data;
using Lightest.Data.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Lightest.Api.Controllers
{
    [Produces("application/json")]
    [Route("[controller]")]
    public class ContestsController : BaseUserController
    {
        public ContestsController(RelationalDbContext context, UserManager<ApplicationUser> userManager) : base(context, userManager)
        {
        }

        [HttpPost("by-name")]
        [ProducesResponseType(200, Type = typeof(IEnumerable<AddToContestView>))]
        [ProducesResponseType(403)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<ActionResult<IEnumerable<AddToContestView>>> AddToContest(AddToContestByNameRequest request)
        {
            if (string.IsNullOrEmpty(request.Pattern))
            {
                return BadRequest(nameof(request.Pattern));
            }

            if (request.ContestId == default)
            {
                return BadRequest(nameof(request.ContestId));
            }

            var category = _context.Categories
                .AsNoTracking().Include(c => c.Users)
                .First(c => c.Id == request.ContestId);

            if (category == null)
            {
                return NotFound(nameof(request.ContestId));
            }

            var users = _context.Users.Where(u => EF.Functions.Like(u.UserName, request.Pattern))
                .Select(u => u.Id);

            var result = new List<AddToContestView>();

            foreach (var userId in users)
            {
                if (category.Users.Any(u => u.UserId == userId))
                {
                    continue;
                }

                category.Users.Add(new CategoryUser
                {
                    UserId = userId,
                    CanRead = true
                });

                result.Add(new AddToContestView
                {
                    UserId = userId
                });
            }

            await _context.SaveChangesAsync();

            return Ok(result);
        }
    }
}
