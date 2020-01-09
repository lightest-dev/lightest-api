using System;
using System.Threading.Tasks;
using Lightest.IdentityServer.RequestModels;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;

namespace Lightest.Tests.IdentityServer.Tests.AccountController
{
    public class Login : BaseTest
    {
        protected readonly LogInRequest _request;

        protected void SetSignInResult(Microsoft.AspNetCore.Identity.SignInResult result) =>
            _signInManager.Setup(m => m.PasswordSignInAsync(_request.Login, _request.Password,
                _request.RememberMe, It.IsAny<bool>()))
                .ReturnsAsync(result);

        public Login()
        {
            _request = new LogInRequest
            {
                Login = "Login",
                Password = "Password",
                RememberMe = true
            };
        }

        [Fact]
        public async Task LoginSuccessful()
        {
            SetSignInResult(Microsoft.AspNetCore.Identity.SignInResult.Success);

            var result = await _controller.Login(_request);

            Assert.IsAssignableFrom<OkResult>(result);
        }

        [Fact]
        public async Task LoginFailed()
        {
            SetSignInResult(Microsoft.AspNetCore.Identity.SignInResult.Failed);

            var result = await _controller.Login(_request);

            Assert.IsAssignableFrom<BadRequestResult>(result);
        }

        [Fact]
        public async Task LoginRequiresTwoFactor()
        {
            SetSignInResult(Microsoft.AspNetCore.Identity.SignInResult.TwoFactorRequired);

            await Assert.ThrowsAsync<NotImplementedException>(() => _controller.Login(_request));
        }

        [Fact]
        public async Task UserIsLocked()
        {
            SetSignInResult(Microsoft.AspNetCore.Identity.SignInResult.LockedOut);

            await Assert.ThrowsAsync<NotImplementedException>(() => _controller.Login(_request));
        }
    }
}
