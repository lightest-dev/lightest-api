using System.Threading.Tasks;
using Lightest.Data.Models;
using Lightest.IdentityServer.RequestModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;

namespace Lightest.Tests.IdentityServer.Tests.AccountController
{
    public class AddToRole : BaseTest
    {
        private readonly AddToRoleRequest _addToRoleRequest;
        private readonly ApplicationUser _user;

        public AddToRole()
        {
            _addToRoleRequest = new AddToRoleRequest
            {
                Role = "Admin",
                UserId = "UserId"
            };

            _user = new ApplicationUser();

            _userManager.Setup(um => um.FindByIdAsync(_addToRoleRequest.UserId))
                .ReturnsAsync(_user);
        }

        protected void SetRoleResult(IdentityResult result) =>
            _userManager.Setup(um => um.AddToRoleAsync(_user, _addToRoleRequest.Role))
                .ReturnsAsync(result);

        [Fact]
        public async Task WrongRole()
        {
            _addToRoleRequest.Role = "WrongRole";

            var result = await _controller.AddToRole(_addToRoleRequest);
            var badRequestResult = result as BadRequestObjectResult;
            Assert.NotNull(badRequestResult);

            var value = badRequestResult.Value as string;
            Assert.Equal(nameof(_addToRoleRequest.Role), value);
        }

        [Fact]
        public async Task UserNotFound()
        {
            _userManager.Setup(um => um.FindByIdAsync(_addToRoleRequest.UserId))
                .ReturnsAsync(default(ApplicationUser));

            var result = await _controller.AddToRole(_addToRoleRequest);
            var badRequestResult = result as BadRequestObjectResult;
            Assert.NotNull(badRequestResult);

            var value = badRequestResult.Value as string;
            Assert.Equal(nameof(_addToRoleRequest.UserId), value);
        }

        [Fact]
        public async Task AddingFailed()
        {
            SetRoleResult(IdentityResult.Failed());

            var result = await _controller.AddToRole(_addToRoleRequest);
            Assert.IsAssignableFrom<BadRequestResult>(result);
        }

        [Fact]
        public async Task AddingSucceeded()
        {
            SetRoleResult(IdentityResult.Success);

            var result = await _controller.AddToRole(_addToRoleRequest);
            Assert.IsAssignableFrom<OkResult>(result);
        }
    }
}
