using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Lightest.Data.Models;
using Lightest.IdentityServer.RequestModels;
using Lightest.IdentityServer.ResponseModels;
using Lightest.IdentityServer.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Lightest.IdentityServer.Controllers
{
    [Produces("application/json")]
    [Route("account/batch")]
    [Authorize]
    public class BatchAccountController : ControllerBase
    {
        private readonly IPasswordGenerator _passwordGenerator;
        private readonly UserManager<ApplicationUser> _userManager;

        public BatchAccountController(IPasswordGenerator passwordGenerator, UserManager<ApplicationUser> userManager)
        {
            _passwordGenerator = passwordGenerator;
            _userManager = userManager;
        }

        [HttpPost]
        [Route("register")]
        public async Task<ActionResult<BatchRegisterResponse>> Register(BatchRegisterRequest request)
        {
            if (request.StartIndex < 0)
            {
                return BadRequest(nameof(request.StartIndex));
            }

            if (request.EndIndex < request.StartIndex)
            {
                return BadRequest(nameof(request.EndIndex));
            }

            if (string.IsNullOrEmpty(request.Prefix))
            {
                return BadRequest(nameof(request.Prefix));
            }

            var numberCount = (int)MathF.Floor(MathF.Log10(request.EndIndex) + 1);
            var format = request.Prefix + "{0:" + new string('0', numberCount) + "}";

            var users = new List<PasswordResponse>(request.EndIndex - request.StartIndex + 1);

            for (var i = request.StartIndex; i <= request.EndIndex; i++)
            {
                var login = string.Format(format, i);
                var password = _passwordGenerator.GeneratePassword();

                var user = new ApplicationUser
                {
                    Id = Guid.NewGuid().ToString(),
                    UserName = login
                };

                var result = await _userManager.CreateAsync(user, password);

                if (result.Succeeded)
                {
                    users.Add(new PasswordResponse
                    {
                        Id = user.Id,
                        Login = user.UserName,
                        Password = password
                    });
                }
                else
                {
                    var errorResponse = new BatchRegisterResponse
                    {
                        Errors = result.Errors,
                        GeneratedPasswords = users
                    };

                    return errorResponse;
                }
            }

            var response = new BatchRegisterResponse
            {
                GeneratedPasswords = users
            };

            return response;

        }
    }
}
