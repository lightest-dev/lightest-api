using System;
using System.Linq;
using System.Threading.Tasks;
using Lightest.Data.Models;
using Lightest.Data.Models.TaskModels;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;

namespace Lightest.Tests.Api.Tests.TasksController
{
    public class DeleteTask : BaseTest
    {
        [Fact]
        public async Task EntryNotFound()
        {
            AddDataToDb();
            await _context.SaveChangesAsync();

            var result = await _controller.DeleteTask(Guid.NewGuid());

            Assert.IsAssignableFrom<NotFoundResult>(result);
            Assert.Equal(1, _context.Categories.Count());
        }

        [Fact]
        public async Task Forbidden()
        {
            AddDataToDb();
            await _context.SaveChangesAsync();

            _accessServiceMock.Setup(m => m.HasWriteAccess(It.IsAny<Guid>(),
                It.Is<ApplicationUser>(u => u.Id == _user.Id)))
                .ReturnsAsync(false);

            var result = await _controller.DeleteTask(_task.Id);

            Assert.IsAssignableFrom<ForbidResult>(result);
            Assert.Equal(1, _context.Categories.Count());
        }

        [Fact]
        public async Task Deleted()
        {
            AddDataToDb();
            await _context.SaveChangesAsync();

            var result = await _controller.DeleteTask(_task.Id);

            var okResult = result as OkObjectResult;
            Assert.NotNull(okResult);

            Assert.Equal(0, _context.Tasks.Count());

            var taskResult = okResult.Value as TaskDefinition;
            Assert.NotNull(taskResult);

            Assert.Equal(_task.Name, taskResult.Name);
            Assert.Equal(_task.CategoryId, taskResult.CategoryId);
            Assert.Equal(_task.Examples, taskResult.Examples);
            Assert.Equal(_task.Description, taskResult.Description);
            Assert.Equal(_task.Points, taskResult.Points);
            Assert.Equal(_task.Public, taskResult.Public);
            Assert.Equal(_task.CheckerId, taskResult.CheckerId);
        }
    }
}
