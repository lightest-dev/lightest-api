using System;
using System.Security.Claims;
using System.Threading.Tasks;
using IdentityServer4.Extensions;
using IdentityServer4.Services;
using Lightest.Data.Models;
using Lightest.IdentityServer.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Lightest.IdentityServer.Controllers
{
    [Produces("application/json")]
    [Route("[controller]")]
    [Authorize]
    public class AccountController : Controller
    {
        private readonly ILogger _logger;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IPersistedGrantService _persistedGrantService;

        public AccountController(
            UserManager<ApplicationUser> userManager,
            IPersistedGrantService persistedGrantService,
            SignInManager<ApplicationUser> signInManager,
            RoleManager<IdentityRole> roleManager,
            ILoggerFactory loggerFactory)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _roleManager = roleManager;
            _persistedGrantService = persistedGrantService;
            _logger = loggerFactory.CreateLogger<AccountController>();
        }

        [HttpPost]
        [AllowAnonymous]
        [Route("login")]
        public async Task<IActionResult> Login([FromBody]LogInViewModel model)
        {
            // This doesn't count login failures towards account lockout
            // To enable password failures to trigger account lockout, set lockoutOnFailure: true
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
        [AllowAnonymous]
        [Route("logout")]
        public async Task<IActionResult> Logout([FromBody]string clientName)
        {
            var subjectId = HttpContext.User.Identity.GetSubjectId();

            // delete authentication cookie
            await _signInManager.SignOutAsync();

            // set this so UI rendering sees an anonymous user
            HttpContext.User = new ClaimsPrincipal(new ClaimsIdentity());

            // get context information (client name, post logout redirect URI and iframe for federated signout)

            await _persistedGrantService.RemoveAllGrantsAsync(subjectId, clientName);
            return Ok();
        }

        [HttpPost]
        [AllowAnonymous]
        [Route("register")]
        public async Task<IActionResult> Register([FromBody] RegisterViewModel model)
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
        public async Task<IActionResult> AddToRole([FromBody] AddToRoleViewModel model)
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
