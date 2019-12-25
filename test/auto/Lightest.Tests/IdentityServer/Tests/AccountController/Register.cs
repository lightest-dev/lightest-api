using System.Threading.Tasks;
using Lightest.Data.Models;
using Lightest.IdentityServer.RequestModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;

namespace Lightest.Tests.IdentityServer.Tests.AccountController
{
    public class Register : BaseTest
    {
        private readonly RegisterRequest _registerRequest;

        public Register()
        {
            _registerRequest = new RegisterRequest
            {
                Email = "email",
                Password = "password",
                UserName = "userName"
            };
        }

        protected void SetRegisterResult(IdentityResult result) =>
            _userManager.Setup(um => um.CreateAsync(It.Is<ApplicationUser>(u => u.Email == _registerRequest.Email &&
                u.UserName == _registerRequest.UserName), _registerRequest.Password))
                .ReturnsAsync(result);

        [Fact]
        public async Task RegisterSuccessful()
        {
            SetRegisterResult(IdentityResult.Success);

            var result = await _controller.Register(_registerRequest);

            Assert.IsAssignableFrom<OkResult>(result);
        }

        [Fact]
        public async Task RegisterFailed()
        {
            SetRegisterResult(IdentityResult.Failed());

            var result = await _controller.Register(_registerRequest);

            Assert.IsAssignableFrom<BadRequestResult>(result);
        }
    }
}
