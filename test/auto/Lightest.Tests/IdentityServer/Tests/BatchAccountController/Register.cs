using System.Linq;
using System.Threading.Tasks;
using Lightest.Data.Models;
using Lightest.IdentityServer.RequestModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;

namespace Lightest.Tests.IdentityServer.Tests.BatchAccountController
{
    public class Register : BaseTest
    {
        [Fact]
        public async Task WrongStartIndex()
        {
            var request = new BatchRegisterRequest
            {
                StartIndex = -1,
                EndIndex = 1,
                Prefix = "q"
            };

            var result = await _controller.Register(request);
            var badRequest = result.Result as BadRequestObjectResult;
            var error = badRequest.Value as string;

            Assert.Equal(nameof(request.StartIndex), error);
        }

        [Fact]
        public async Task WrongEndIndex()
        {
            var request = new BatchRegisterRequest
            {
                StartIndex = 2,
                EndIndex = 1,
                Prefix = "q"
            };

            var result = await _controller.Register(request);
            var badRequest = result.Result as BadRequestObjectResult;
            var error = badRequest.Value as string;

            Assert.Equal(nameof(request.EndIndex), error);
        }

        [Theory]
        [InlineData("")]
        [InlineData(default(string))]
        public async Task WrongPrefix(string prefix)
        {
            var request = new BatchRegisterRequest
            {
                StartIndex = 1,
                EndIndex = 2,
                Prefix = prefix
            };

            var result = await _controller.Register(request);
            var badRequest = result.Result as BadRequestObjectResult;
            var error = badRequest.Value as string;

            Assert.Equal(nameof(request.Prefix), error);
        }

        [Fact]
        public async Task RegistrationFailed()
        {
            var password = "qwe";
            _passwordGenerator.Setup(g => g.GeneratePassword())
                .Returns(password);

            var identityError = new IdentityError();

            _userManager.SetupSequence(um => um.CreateAsync(It.IsAny<ApplicationUser>(), It.IsAny<string>()))
                .ReturnsAsync(IdentityResult.Success)
                .ReturnsAsync(IdentityResult.Success)
                .ReturnsAsync(IdentityResult.Failed(identityError))
                .ReturnsAsync(IdentityResult.Success);

            var request = new BatchRegisterRequest
            {
                StartIndex = 1,
                EndIndex = 10,
                Prefix = "pr"
            };

            var result = await _controller.Register(request);
            var registeredUsers = result.Value.GeneratedPasswords;
            var errors = result.Value.Errors;

            Assert.Single(errors);
            Assert.Equal(identityError, errors.Single());

            Assert.Equal(2, registeredUsers.Count());
            Assert.Equal(2, result.Value.GeneratedCount);
            var users = registeredUsers.ToArray();

            Assert.Equal("pr01", users[0].UserName);
            Assert.Equal(password, users[0].Password);
            Assert.Equal("pr02", users[1].UserName);
            Assert.Equal(password, users[1].Password);
        }

        [Fact]
        public async Task RegistrationSuccessful()
        {
            var password = "qwe";
            _passwordGenerator.Setup(g => g.GeneratePassword())
                .Returns(password);

            _userManager.Setup(um => um.CreateAsync(It.IsAny<ApplicationUser>(), It.IsAny<string>()))
                .ReturnsAsync(IdentityResult.Success);

            var request = new BatchRegisterRequest
            {
                StartIndex = 0,
                EndIndex = 10,
                Prefix = "pr"
            };

            var result = await _controller.Register(request);
            var registeredUsers = result.Value.GeneratedPasswords;

            Assert.Equal(11, registeredUsers.Count());
            Assert.Equal(11, result.Value.GeneratedCount);
            foreach (var user in registeredUsers)
            {
                Assert.Equal(4, user.UserName.Length);
                Assert.StartsWith(request.Prefix, user.UserName);

                Assert.Equal(password, user.Password);
            }
        }
    }
}
