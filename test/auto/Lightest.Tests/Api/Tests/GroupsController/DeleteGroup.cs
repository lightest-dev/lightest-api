using System;
using System.Linq;
using System.Threading.Tasks;
using Lightest.Data.Models;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;

namespace Lightest.Tests.Api.Tests.GroupsController
{
    public class DeleteGroup : BaseTest
    {
        [Fact]
        public async Task EntryNotFound()
        {
            AddDataToDb();
            await _context.SaveChangesAsync();

            var result = await _controller.DeleteGroup(Guid.NewGuid());

            Assert.IsAssignableFrom<NotFoundResult>(result);
            Assert.Equal(3, _context.Groups.Count());
        }

        [Fact]
        public async Task Forbidden()
        {
            AddDataToDb();
            await _context.SaveChangesAsync();

            _accessServiceMock.Setup(m => m.CheckWriteAccess(It.IsAny<Group>(),
                It.Is<ApplicationUser>(u => u.Id == _user.Id)))
                .Returns(false);

            var result = await _controller.DeleteGroup(_child2.Id);

            Assert.IsAssignableFrom<ForbidResult>(result);
            Assert.Equal(3, _context.Groups.Count());
        }

        [Fact]
        public async Task Deleted()
        {
            AddDataToDb();
            await _context.SaveChangesAsync();

            var result = await _controller.DeleteGroup(_child2.Id);

            var okResult = result as OkObjectResult;
            Assert.NotNull(okResult);

            var groupResult = okResult.Value as Group;
            Assert.NotNull(groupResult);

            Assert.Equal(_child2.Name, groupResult.Name);
            Assert.Equal(_child2.ParentId, groupResult.ParentId);
            Assert.Equal(_child2.Id, groupResult.Id);

            Assert.Equal(2, _context.Groups.Count());
        }
    }
}
