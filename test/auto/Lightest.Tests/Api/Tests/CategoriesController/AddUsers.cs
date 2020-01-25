using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Lightest.Api.Models;
using Lightest.Data.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Moq;
using Xunit;

namespace Lightest.Tests.Api.Tests.CategoriesController
{
    public class AddUsers : BaseTest
    {
        [Fact]
        public async Task Forbidden()
        {
            AddDataToDb();
            await _context.SaveChangesAsync();

            _accessServiceMock.Setup(m => m.HasWriteAccess(It.IsAny<Guid>(),
                It.Is<ApplicationUser>(u => u.Id == _user.Id)))
                .ReturnsAsync(false);

            var result = await _controller.AddUsers(_child1.Id, new List<AccessRights>());

            Assert.IsAssignableFrom<ForbidResult>(result);
        }

        [Fact]
        public async Task UserAlreadyAssigned()
        {
            AddDataToDb();
            await _context.SaveChangesAsync();

            var result = await _controller.AddUsers(_child1.Id, new List<AccessRights>
            {
                new AccessRights
                {
                    UserId = _user.Id,
                    CanChangeAccess = true,
                    CanRead = true,
                    CanWrite = true,
                    IsOwner = true
                }
            });

            Assert.IsAssignableFrom<OkResult>(result);
            var category = _context.Categories
                .Include(c => c.Users)
                .Single(c => c.Id == _child1.Id);
            Assert.Single(category.Users);
            var user = category.Users.First();
            Assert.True(user.CanChangeAccess);
            Assert.True(user.CanRead);
            Assert.True(user.CanWrite);
            Assert.False(user.IsOwner);
        }

        [Fact]
        public async Task UserNotAssigned()
        {
            AddDataToDb();
            await _context.SaveChangesAsync();

            var result = await _controller.AddUsers(_child2.Id, new List<AccessRights>
            {
                new AccessRights
                {
                    UserId = _user.Id,
                    CanChangeAccess = true,
                    CanRead = true,
                    CanWrite = true,
                    IsOwner = true
                }
            });

            Assert.IsAssignableFrom<OkResult>(result);
            var category = _context.Categories
                .Include(c => c.Users)
                .Single(c => c.Id == _child2.Id);
            Assert.Single(category.Users);
            var user = category.Users.First();
            Assert.True(user.CanChangeAccess);
            Assert.True(user.CanRead);
            Assert.True(user.CanWrite);
            Assert.False(user.IsOwner);
        }

        [Fact]
        public async Task NotFound()
        {
            AddDataToDb();
            await _context.SaveChangesAsync();

            var result = await _controller.AddUsers(Guid.NewGuid(), new List<AccessRights>());
            Assert.IsAssignableFrom<NotFoundResult>(result);
        }

        [Fact]
        public async Task UserNotFound()
        {
            AddDataToDb();
            await _context.SaveChangesAsync();

            var result = await _controller.AddUsers(_child1.Id, new List<AccessRights>
            {
                new AccessRights
                {
                    UserId = Guid.NewGuid().ToString()
                }
            });
            Assert.IsAssignableFrom<BadRequestResult>(result);
        }
    }
}
