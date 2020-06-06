using System;
using System.Security.Claims;
using System.Threading.Tasks;
using IdentityServer4.Extensions;
using IdentityServer4.Services;
using Lightest.Data.Models;
using Lightest.IdentityServer.RequestModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Lightest.IdentityServer.Controllers
{
    [Produces("application/json")]
    [Route("[controller]")]
    [Authorize]
    public class AccountController : Controller
    {
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IPersistedGrantService _persistedGrantService;

        public AccountController(
            UserManager<ApplicationUser> userManager,
            IPersistedGrantService persistedGrantService,
            SignInManager<ApplicationUser> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _persistedGrantService = persistedGrantService;
        }

        [HttpPost]
        [AllowAnonymous]
        [Route("login")]
        public async Task<IActionResult> Login([FromBody] LogInRequest model)
        {
            // This doesn't count login failures towards account lockout To enable password failures
            // to trigger account lockout, set lockoutOnFailure: true
            var result = await _signInManager.PasswordSignInAsync(model.Login, model.Password, model.RememberMe, lockoutOnFailure: false);
            if (result.Succeeded)
            {
                return Ok();
            }
            if (result.RequiresTwoFactor)
            {
                throw new NotImplementedException();
            }
            if (result.IsLockedOut)
            {
                throw new NotImplementedException();
            }
            else
            {
                return BadRequest();
            }

            // If we got this far, something failed, redisplay form
        }

        [HttpPost]
        [Route("logout")]
        public async Task<IActionResult> Logout([FromBody] LogOutRequest model)
        {
            var subjectId = HttpContext.User.Identity.GetSubjectId();

            // delete authentication cookie
            await _signInManager.SignOutAsync();

            // set this so UI rendering sees an anonymous user
            HttpContext.User = new ClaimsPrincipal(new ClaimsIdentity());

            // get context information (client name, post logout redirect URI and iframe for
            // federated signout)

            await _persistedGrantService.RemoveAllGrantsAsync(subjectId, model.ClientName);
            return Ok();
        }

        [HttpPost]
        [AllowAnonymous]
        [Route("register")]
        public async Task<IActionResult> Register([FromBody] RegisterRequest model)
        {
            var user = new ApplicationUser
            {
                UserName = model.UserName,
                Email = model.Email
            };
            var result = await _userManager.CreateAsync(user, model.Password);
            if (result.Succeeded)
            {
                return Ok();
            }

            return BadRequest();
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        [Route("role")]
        public async Task<IActionResult> AddToRole([FromBody] AddToRoleRequest model)
        {
            if (model.Role != "Admin" && model.Role != "Teacher")
            {
                return BadRequest(nameof(model.Role));
            }

            var user = await _userManager.FindByIdAsync(model.UserId);

            if (user == null)
            {
                return BadRequest(nameof(model.UserId));
            }

            var result = await _userManager.AddToRoleAsync(user, model.Role);

            if (result.Succeeded)
            {
                return Ok();
            }

            return BadRequest();
        }
    }
}
