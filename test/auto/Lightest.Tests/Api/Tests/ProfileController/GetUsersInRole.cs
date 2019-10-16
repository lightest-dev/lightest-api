using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Lightest.Data.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;

namespace Lightest.Tests.Api.Tests.ProfileController
{
    public class GetUsersInRole : BaseTest
    {
        private readonly ApplicationUser _secondUser;
        private readonly IdentityRole _role;
        private readonly IdentityUserRole<string> _userRole;

        public GetUsersInRole()
        {
            _secondUser = new ApplicationUser
            {
                Id = Guid.NewGuid().ToString()
            };

            _role = new IdentityRole
            {
                Id = Guid.NewGuid().ToString(),
                Name = "role",
                NormalizedName = "ROLE"
            };

            _userRole = new IdentityUserRole<string>
            {
                RoleId = _role.Id,
                UserId = _secondUser.Id
            };
        }

        [Fact]
        public async Task Forbidden()
        {
            _accessServiceMock.Setup(m => m.CheckWriteAccess(It.IsAny<ApplicationUser>(),
                It.Is<ApplicationUser>(u => u.Id == _user.Id)))
                .Returns(false);

            var result = await _controller.GetUsersInRole("qwe");

            Assert.IsAssignableFrom<ForbidResult>(result);
        }

        [Fact]
        public async Task IdsReturned()
        {
            _context.Users.Add(_user);
            _context.Users.Add(_secondUser);
            _context.Roles.Add(_role);
            _context.UserRoles.Add(_userRole);
            await _context.SaveChangesAsync();

            var result = await _controller.GetUsersInRole("rOlE");
            var okResult = result as OkObjectResult;
            Assert.NotNull(okResult);

            var users = okResult.Value as IEnumerable<string>;
            Assert.NotNull(users);
            Assert.Single(users);

            var userId = users.First();
            Assert.Equal(_secondUser.Id, userId);
        }

        [Fact]
        public async Task RoleNotFound()
        {
            _context.Users.Add(_user);
            _context.Users.Add(_secondUser);
            _context.Roles.Add(_role);
            _context.UserRoles.Add(_userRole);
            await _context.SaveChangesAsync();

            var result = await _controller.GetUsersInRole("qwe");
            Assert.IsAssignableFrom<NotFoundResult>(result);
        }
    }
}
