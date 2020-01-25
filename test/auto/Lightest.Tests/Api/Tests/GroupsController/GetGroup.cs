using System;
using System.Linq;
using System.Threading.Tasks;
using Lightest.Api.ResponseModels.GroupViews;
using Lightest.Data.Models;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;

namespace Lightest.Tests.Api.Tests.GroupsController
{
    public class GetGroup : BaseTest
    {
        [Fact]
        public async Task NoAccess()
        {
            AddDataToDb();
            await _context.SaveChangesAsync();

            _accessServiceMock.Setup(m => m.HasReadAccess(It.IsAny<Guid>(),
                It.Is<ApplicationUser>(u => u.Id == _user.Id)))
                .ReturnsAsync(false);

            var result = await _controller.GetGroup(_parent.Id);
            Assert.IsAssignableFrom<ForbidResult>(result);
        }

        [Fact]
        public async Task NotFound()
        {
            AddDataToDb();
            await _context.SaveChangesAsync();

            var result = await _controller.GetGroup(Guid.NewGuid());
            Assert.IsAssignableFrom<NotFoundResult>(result);
        }

        [Fact]
        public async Task ParentCategoryFound()
        {
            AddDataToDb();
            await _context.SaveChangesAsync();

            var result = await _controller.GetGroup(_parent.Id);

            var okResult = result as OkObjectResult;
            Assert.NotNull(okResult);

            var groupResult = okResult.Value as CompleteGroupView;

            Assert.NotNull(groupResult);
            Assert.Equal(_parent.Id, groupResult.Id);
            Assert.Equal(_parent.Name, groupResult.Name);
            Assert.Equal(2, groupResult.SubGroups.Count());

            Assert.Empty(groupResult.Users);
        }

        [Fact]
        public async Task ChildCategoryFound()
        {
            AddDataToDb();
            await _context.SaveChangesAsync();

            var result = await _controller.GetGroup(_child2.Id);

            var okResult = result as OkObjectResult;
            Assert.NotNull(okResult);

            var groupResult = okResult.Value as CompleteGroupView;

            Assert.NotNull(groupResult);
            Assert.Equal(_child2.Id, groupResult.Id);
            Assert.Equal(_child2.Name, groupResult.Name);
            Assert.Empty(groupResult.SubGroups);

            Assert.Single(groupResult.Users);
            var user = groupResult.Users.First();
            Assert.Equal(_user.Id, user.Id);
            Assert.Equal(_user.UserName, user.UserName);
        }

        [Fact]
        public async Task ChildCategoryFoundWithNoWrite()
        {
            AddDataToDb();
            await _context.SaveChangesAsync();
            _accessServiceMock.Setup(m => m.HasWriteAccess(It.IsAny<Guid>(),
                It.Is<ApplicationUser>(u => u.Id == _user.Id)))
                .ReturnsAsync(false);

            var result = await _controller.GetGroup(_child2.Id);

            var okResult = result as OkObjectResult;
            Assert.NotNull(okResult);

            var groupResult = okResult.Value as CompleteGroupView;

            Assert.NotNull(groupResult);
            Assert.Equal(_child2.Id, groupResult.Id);
            Assert.Equal(_child2.Name, groupResult.Name);
            Assert.Empty(groupResult.SubGroups);

            Assert.Null(groupResult.Users);
        }
    }
}
