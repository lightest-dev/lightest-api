using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using IdentityModel;
using IdentityServer4;
using IdentityServer4.Extensions;
using IdentityServer4.Services;
using IdentityServer4.Stores;
using Lightest.IdentityServer.ViewModels;
using Lightest.Data.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Lightest.IdentityServer.Controllers
{
    [Produces("application/json")]
    [Route("api/[controller]")]
    [Authorize]
    public class AccountController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly ILogger _logger;
        private readonly IIdentityServerInteractionService _interaction;
        private readonly IClientStore _clientStore;
        private readonly IPersistedGrantService _persistedGrantService;

        public AccountController(
            UserManager<ApplicationUser> userManager,
            IPersistedGrantService persistedGrantService,
            SignInManager<ApplicationUser> signInManager,
            ILoggerFactory loggerFactory,
            IIdentityServerInteractionService interaction,
            IClientStore clientStore)
        {
            _userManager = userManager;
            _persistedGrantService = persistedGrantService;
            _signInManager = signInManager;
            _logger = loggerFactory.CreateLogger<AccountController>();
            _interaction = interaction;
            _clientStore = clientStore;
        }

        [HttpPost]
        [AllowAnonymous]
        [Route("Login")]
        public async Task<IActionResult> Login([FromBody]LogInViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }

            if (ModelState.IsValid)
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
            }

            // If we got this far, something failed, redisplay form
            return BadRequest();
        }

        [HttpPost]
        [AllowAnonymous]
        [Route("Logout")]
        public async Task<IActionResult> Logout([FromBody]string clientName)
        {
            var idp = User?.FindFirst(JwtClaimTypes.IdentityProvider)?.Value;
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
        [Route("Register")]
        public async Task<IActionResult> Register([FromBody] RegisterViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }
            var user = new ApplicationUser
            {
                UserName = model.UserName,
                Email = model.Email
            };
            var result = await _userManager.CreateAsync(user, model.Password);
            return Ok();
        }
    }
}