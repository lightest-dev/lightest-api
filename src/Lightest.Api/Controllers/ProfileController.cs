using System.Linq;
using System.Threading.Tasks;
using Lightest.Api.Models;
using Lightest.Api.Services.AccessServices;
using Lightest.Api.ResponseModels;
using Lightest.Data;
using Lightest.Data.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Lightest.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ProfileController : Controller
    {
        private readonly RelationalDbContext _context;


        private readonly IAccessService<ApplicationUser> _accessService;

        public ProfileController(RelationalDbContext context,
            IAccessService<ApplicationUser> accessService)
        {
            _context = context;
            _accessService = accessService;
        }

        [HttpGet]
        [ProducesResponseType(200, Type = typeof(ApplicationUser))]
        [ProducesResponseType(403)]
        public async Task<IActionResult> GetUsers()
        {
            var user = await GetCurrentUser();
            if (!_accessService.CheckAdminAccess(null, user))
            {
                return Forbid();
            }
            return Ok(_context.Users);
        }

        [HttpGet("{id}")]
        [ProducesResponseType(200, Type = typeof(CompleteUser))]
        [ProducesResponseType(400)]
        [ProducesResponseType(403)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> GetUser([FromRoute] string id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var currentUser = await GetCurrentUser();

            var requestedUser = await _context.Users
                .Include(u => u.Groups)
                .ThenInclude(g => g.Group)
                .Include(u => u.Tasks)
                .ThenInclude(t => t.Task)
                .SingleOrDefaultAsync(u => u.Id == id);

            if (requestedUser == null)
            {
                return NotFound();
            }

            if (!_accessService.CheckReadAccess(requestedUser, currentUser))
            {
                return Forbid();
            }

            return Ok(new CompleteUser
            {
                Name = requestedUser.Name,
                Surname = requestedUser.Surname,
                Email = requestedUser.Email,
                Login = requestedUser.UserName,
                Tasks = requestedUser.Tasks.Select(t => new UserTaskViewModel
                {
                    Id = t.Task.Id,
                    Name = t.Task.Name,
                    Completed = t.Completed,
                    HighScore = t.HighScore
                }),
                Groups = requestedUser.Groups.Select(g => new BasicNameViewModel
                {
                    Id = g.GroupId,
                    Name = g.Group.Name
                })
            });
        }

        [HttpPut("{id}")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(403)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> PutUser([FromRoute] string id, [FromBody]PersonalDataRequest personalData)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != personalData.UserId)
            {
                return BadRequest();
            }

            var currentUser = await GetCurrentUser();

            var requestedUser = await _context.Users.FirstOrDefaultAsync(u => u.Id == id);

            if (requestedUser == null)
            {
                return NotFound();
            }

            if (!_accessService.CheckWriteAccess(requestedUser, currentUser))
            {
                return Forbid();
            }

            requestedUser.Name = personalData.Name;
            requestedUser.Surname = personalData.Surname;
            var result = await _context.SaveChangesAsync();

            return Ok();
        }

        private async Task<ApplicationUser> GetCurrentUser()
        {
            var id = User.Claims.SingleOrDefault(c => c.Type == "sub");
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == id.Value);
            return user;
        }
    }
}
