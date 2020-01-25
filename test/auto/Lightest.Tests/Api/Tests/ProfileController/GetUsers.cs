using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Lightest.Api.ResponseModels.UserViews;
using Lightest.Data.Models;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;

namespace Lightest.Tests.Api.Tests.ProfileController
{
    public class GetUsers : BaseTest
    {
        [Fact]
        public async Task Forbidden()
        {
            _accessServiceMock.Setup(m => m.CanEdit(It.IsAny<Guid>(),
                It.Is<ApplicationUser>(u => u.Id == _user.Id)))
                .ReturnsAsync(false);

            var result = await _controller.GetUsers(new Sieve.Models.SieveModel());

            Assert.IsAssignableFrom<ForbidResult>(result);
        }

        [Fact]
        public async Task UsersReturned()
        {
            _context.Users.Add(_user);
            var user = new ApplicationUser
            {
                Id = Guid.NewGuid().ToString()
            };
            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            var result = await _controller.GetUsers(new Sieve.Models.SieveModel());
            var okResult = result as OkObjectResult;
            Assert.NotNull(okResult);

            var users = okResult.Value as IEnumerable<ProfileView>;
            Assert.NotNull(users);
            Assert.Equal(2, users.Count());
        }
    }
}
