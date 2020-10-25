using System.Threading.Tasks;
using Lightest.Data.Models;
using Lightest.IdentityServer.RequestModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;

namespace Lightest.Tests.IdentityServer.Tests.AccountController
{
    public class ResetPassword : BaseTest
    {
        protected const string GeneratedPassword = "password";
        protected readonly ResetPasswordRequest _request;
        protected readonly ApplicationUser _user;

        public ResetPassword()
        {
            _request = new ResetPasswordRequest
            {
                UserName = "sample_login",
            };

            _user = new ApplicationUser
            {
                UserName = _request.UserName,
                Id = "id"
            };

            _userManager.Setup(um => um.FindByNameAsync(_user.UserName))
                .ReturnsAsync(_user);
        }

        private void SetPasswordResetResult(IdentityResult result)
        {
            var token = "token";
            _userManager.Setup(um => um.GeneratePasswordResetTokenAsync(_user))
                .ReturnsAsync(token);
            _passwordGenerator.Setup(pg => pg.GeneratePassword())
                .Returns(GeneratedPassword);

            _userManager.Setup(um => um.ResetPasswordAsync(_user, token, GeneratedPassword))
                .ReturnsAsync(result);
        }

        [Fact]
        public async Task PasswordResetSuccessfully()
        {
            var resetResult = IdentityResult.Success;
            SetPasswordResetResult(resetResult);

            var result = await _controller.ResetPassword(_request);
            Assert.NotNull(result.Value);

            Assert.Equal(_user.Id, result.Value.Id);
            Assert.Equal(_user.UserName, result.Value.UserName);
            Assert.Equal(GeneratedPassword, result.Value.Password);
        }

        [Fact]
        public async Task PasswordResetFailed()
        {
            var resetResult = IdentityResult.Failed(
                new IdentityError { Code = "c1", Description = "d1" },
                new IdentityError { Code = "c2", Description = "d2" });
            SetPasswordResetResult(resetResult);

            var result = await _controller.ResetPassword(_request);
            Assert.NotNull(result.Result);

            var badRequestResult = result.Result as BadRequestObjectResult;
            Assert.NotNull(badRequestResult);

            var state = badRequestResult.Value as SerializableError;
            Assert.NotNull(state);

            Assert.Equal(2, state.Count);
            Assert.Equal(new []{ "d1" }, state["c1"]);
            Assert.Equal(new[] { "d2" }, state["c2"]);
        }

        [Fact]
        public async Task UserNotFound()
        {
            _userManager.Setup(um => um.FindByNameAsync(_user.UserName))
                .ReturnsAsync(default(ApplicationUser));

            var result = await _controller.ResetPassword(_request);
            Assert.NotNull(result.Result);

            var notFoundResult = result.Result as NotFoundResult;
            Assert.NotNull(notFoundResult);
        }
    }
}
