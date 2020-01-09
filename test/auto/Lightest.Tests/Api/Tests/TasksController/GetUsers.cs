using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Lightest.Api.ResponseModels;
using Lightest.Data.Models;
using Lightest.Data.Models.TaskModels;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;

namespace Lightest.Tests.Api.Tests.TasksController
{
    public class GetUsers : BaseTest
    {
        [Fact]
        public async Task HasAccess()
        {
            AddDataToDb();
            await _context.SaveChangesAsync();

            var result = await _controller.GetUsers(_task.Id);

            var okResult = result as OkObjectResult;
            Assert.NotNull(okResult);

            var users = okResult.Value as IEnumerable<AccessRightsUser>;
            Assert.NotNull(users);
            Assert.Single(users);

            var user = users.First();
            Assert.Equal(_user.Id, user.Id);
            Assert.True(user.IsOwner);
            Assert.True(user.CanChangeAccess);
            Assert.True(user.CanRead);
            Assert.True(user.CanWrite);
        }

        [Fact]
        public async Task NoReadAccess()
        {
            AddDataToDb();
            await _context.SaveChangesAsync();

            _accessServiceMock.Setup(m => m.HasWriteAccess(It.IsAny<TaskDefinition>(),
                It.Is<ApplicationUser>(u => u.Id == _user.Id)))
                .Returns(false);

            var result = await _controller.GetUsers(_task.Id);

            Assert.IsAssignableFrom<ForbidResult>(result);
        }

        [Fact]
        public async Task NotFound()
        {
            AddDataToDb();
            await _context.SaveChangesAsync();

            var result = await _controller.GetUsers(Guid.NewGuid());

            Assert.IsAssignableFrom<NotFoundResult>(result);
        }
    }
}
