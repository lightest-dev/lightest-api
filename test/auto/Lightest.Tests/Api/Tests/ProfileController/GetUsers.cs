using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Lightest.Api.ResponseModels;
using Lightest.Data.Models;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;

namespace Lightest.Tests.Api.Tests.ProfileController
{
    public class GetUsers: BaseTest
    {
        [Fact]
        public async Task Forbidden()
        {
            _accessServiceMock.Setup(m => m.CheckWriteAccess(It.IsAny<ApplicationUser>(),
                It.Is<ApplicationUser>(u => u.Id == _user.Id)))
                .Returns(false);

            var result = await _controller.GetUsers();

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

            var result = await _controller.GetUsers();
            var okResult = result as OkObjectResult;
            Assert.NotNull(okResult);

            var users = okResult.Value as IEnumerable<ProfileViewModel>;
            Assert.NotNull(users);
            Assert.Equal(2, users.Count());
        }
    }
}
