using System.Security.Claims;
using System.Threading.Tasks;
using Lightest.Data.Models;
using Lightest.IdentityServer.RequestModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;

namespace Lightest.Tests.IdentityServer.Tests.AccountController
{
    public class ChangePassword : BaseTest
    {
        protected const string NewPassword = "password";
        protected readonly ChangePasswordRequest _request;
        protected readonly ApplicationUser _user;

        public ChangePassword()
        {
            _request = new ChangePasswordRequest
            {
                NewPassword = NewPassword,
            };

            _user = new ApplicationUser
            {
                UserName = "login",
                Id = "id"
            };

            _userManager.Setup(um => um.GetUserAsync(It.IsAny<ClaimsPrincipal>()))
                .ReturnsAsync(_user);
        }

        private void SetPasswordResetResult(IdentityResult result)
        {
            var token = "token";
            _userManager.Setup(um => um.GeneratePasswordResetTokenAsync(_user))
                .ReturnsAsync(token);
            _passwordGenerator.Setup(pg => pg.GeneratePassword())
                .Returns(NewPassword);

            _userManager.Setup(um => um.ResetPasswordAsync(_user, token, NewPassword))
                .ReturnsAsync(result);
        }

        [Fact]
        public async Task PasswordChangedSuccessfully()
        {
            var resetResult = IdentityResult.Success;
            SetPasswordResetResult(resetResult);

            var result = await _controller.ChangePassword(_request);

            Assert.IsAssignableFrom<OkResult>(result);
        }

        [Fact]
        public async Task PasswordChangeFailed()
        {
            var resetResult = IdentityResult.Failed(
                new IdentityError { Code = "c1", Description = "d1" },
                new IdentityError { Code = "c2", Description = "d2" });
            SetPasswordResetResult(resetResult);

            var result = await _controller.ChangePassword(_request);
            Assert.NotNull(result);

            var badRequestResult = result as BadRequestObjectResult;
            Assert.NotNull(badRequestResult);

            var state = badRequestResult.Value as SerializableError;
            Assert.NotNull(state);

            Assert.Equal(2, state.Count);
            Assert.Equal(new []{ "d1" }, state["c1"]);
            Assert.Equal(new[] { "d2" }, state["c2"]);
        }
    }
}
