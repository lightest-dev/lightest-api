using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Lightest.AccessService.Interfaces;
using Lightest.Api.RequestModels.UserRequests;
using Lightest.Api.ResponseModels;
using Lightest.Api.ResponseModels.UserViews;
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
    public class ProfileController : BaseUserController
    {
        private readonly IAccessService<ApplicationUser> _accessService;
        private readonly IRoleHelper _roleHelper;
        private readonly ISieveProcessor _sieveProcessor;

        public ProfileController(RelationalDbContext context,
            IAccessService<ApplicationUser> accessService,
            IRoleHelper roleHelper,
            UserManager<ApplicationUser> userManager,
            ISieveProcessor sieveProcessor) : base(context, userManager)
        {
            _accessService = accessService;
            _sieveProcessor = sieveProcessor;
            _roleHelper = roleHelper;
        }

        [HttpGet("all")]
        [ProducesResponseType(200, Type = typeof(IEnumerable<ProfileView>))]
        [ProducesResponseType(403)]
        public async Task<IActionResult> GetUsers([FromQuery] SieveModel sieveModel)
        {
            var user = await GetCurrentUser();

            if (!await _roleHelper.IsTeacher(user))
            {
                return Forbid();
            }

            var users = _sieveProcessor.Apply(sieveModel, _context.Users);

            return Ok(users.Select(u => new ProfileView
            {
                Id = u.Id,
                Email = u.Email,
                Name = u.Name,
                Surname = u.Surname,
                UserName = u.UserName
            }));
        }

        [HttpGet("role/{roleName}")]
        [ProducesResponseType(200, Type = typeof(IEnumerable<string>))]
        [ProducesResponseType(403)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> GetUsersInRole(string roleName)
        {
            var user = await GetCurrentUser();
            if (!await _roleHelper.IsTeacher(user))
            {
                return Forbid();
            }
            var normalizedName = roleName.Normalize().ToUpper();
            var role = await _context.Roles.Where(r => r.NormalizedName == normalizedName).SingleOrDefaultAsync();

            if (role == null)
            {
                return NotFound();
            }

            var userIds = _context.UserRoles.Where(r => r.RoleId == role.Id)
                .Select(r => r.UserId);

            return Ok(userIds);
        }

        [HttpGet("{id}")]
        [ProducesResponseType(200, Type = typeof(CompleteUserView))]
        [ProducesResponseType(400)]
        [ProducesResponseType(403)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> GetUser([FromRoute] string id)
        {
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

            if (!await _accessService.CanRead(Guid.Parse(requestedUser.Id), currentUser))
            {
                return Forbid();
            }

            return Ok(new CompleteUserView
            {
                Name = requestedUser.Name,
                Surname = requestedUser.Surname,
                Email = requestedUser.Email,
                Login = requestedUser.UserName,
                Groups = requestedUser.Groups.Select(g => new BasicNameView
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
        public async Task<IActionResult> PutUser([FromRoute] string id, [FromBody] PersonalDataRequest personalData)
        {
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

            if (!await _accessService.CanEdit(Guid.Parse(requestedUser.Id), currentUser))
            {
                return Forbid();
            }

            requestedUser.Name = personalData.Name;
            requestedUser.Surname = personalData.Surname;
            await _context.SaveChangesAsync();

            return Ok();
        }
    }
}
