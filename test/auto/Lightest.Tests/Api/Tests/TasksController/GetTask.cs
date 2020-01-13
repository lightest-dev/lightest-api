using System;
using System.Threading.Tasks;
using Lightest.Api.ResponseModels.TaskViews;
using Lightest.Data.Models;
using Lightest.Data.Models.TaskModels;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;

namespace Lightest.Tests.Api.Tests.TasksController
{
    public class GetTask : BaseTest
    {
        [Fact]
        public async Task HasWriteAccess()
        {
            AddDataToDb();
            await _context.SaveChangesAsync();

            var result = await _controller.GetTask(_task.Id);

            var okResult = result as OkObjectResult;
            Assert.NotNull(okResult);

            var task = okResult.Value as CompleteTaskView;
            Assert.NotNull(task);

            Assert.Single(task.Tests);
            Assert.Equal(_task.CheckerId, task.Checker.Id);
            Assert.Single(task.Languages);
            Assert.Equal(_task.Id, task.Id);
            Assert.Equal(_task.Name, task.Name);
            Assert.Equal(_task.Points, task.Points);
            Assert.Equal(_task.Public, task.Public);
            Assert.Equal(_task.Examples, task.Examples);
            Assert.Equal(_task.Description, task.Description);
            Assert.Equal(_task.Category.Id, task.Category.Id);
        }

        [Fact]
        public async Task HasReadAccess()
        {
            AddDataToDb();
            await _context.SaveChangesAsync();

            _accessServiceMock.Setup(m => m.HasWriteAccess(It.IsAny<TaskDefinition>(),
                It.Is<ApplicationUser>(u => u.Id == _user.Id)))
                .Returns(false);

            var result = await _controller.GetTask(_task.Id);

            var okResult = result as OkObjectResult;
            Assert.NotNull(okResult);

            var task = okResult.Value as CompleteTaskView;
            Assert.NotNull(task);

            Assert.Null(task.Tests);
            Assert.Null(task.Checker);
            Assert.Single(task.Languages);

            Assert.Equal(_task.Id, task.Id);
            Assert.Equal(_task.Name, task.Name);
            Assert.Equal(_task.Points, task.Points);
            Assert.Equal(_task.Public, task.Public);
            Assert.Equal(_task.Examples, task.Examples);
            Assert.Equal(_task.Description, task.Description);
            Assert.Equal(_task.Category.Id, task.Category.Id);
        }

        [Fact]
        public async Task NoReadAccess()
        {
            AddDataToDb();
            await _context.SaveChangesAsync();

            _accessServiceMock.Setup(m => m.HasReadAccess(It.IsAny<Guid>(),
                It.Is<ApplicationUser>(u => u.Id == _user.Id)))
                .ReturnsAsync(false);

            var result = await _controller.GetTask(_task.Id);

            Assert.IsAssignableFrom<ForbidResult>(result);
        }

        [Fact]
        public async Task NotFound()
        {
            AddDataToDb();
            await _context.SaveChangesAsync();

            var result = await _controller.GetTask(Guid.NewGuid());

            Assert.IsAssignableFrom<NotFoundResult>(result);
        }
    }
}
