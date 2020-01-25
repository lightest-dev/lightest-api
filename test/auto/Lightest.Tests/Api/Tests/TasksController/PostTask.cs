using System;
using System.Linq;
using System.Threading.Tasks;
using Lightest.Data.Models;
using Lightest.Data.Models.TaskModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Moq;
using Xunit;

namespace Lightest.Tests.Api.Tests.TasksController
{
    public class PostTask : BaseTest
    {
        protected override void AddDataToDb()
        {
            _context.Languages.Add(_language);
            _context.Checkers.Add(_checker);
            _context.Categories.Add(_category);
        }

        [Fact]
        public async Task Forbidden()
        {
            AddDataToDb();
            await _context.SaveChangesAsync();

            _accessServiceMock.Setup(m => m.HasWriteAccess(It.IsAny<Guid>(),
                It.Is<ApplicationUser>(u => u.Id == _user.Id)))
                .ReturnsAsync(false);

            var result = await _controller.PostTask(_task);

            Assert.IsAssignableFrom<ForbidResult>(result);
            Assert.Equal(0, _context.Tasks.Count());
        }

        [Fact]
        public async Task Created()
        {
            AddDataToDb();
            await _context.SaveChangesAsync();

            var result = await _controller.PostTask(_task);
            var createdAtResult = result as CreatedAtActionResult;
            Assert.NotNull(createdAtResult);

            var taskResult = createdAtResult.Value as TaskDefinition;
            Assert.NotNull(taskResult);
            Assert.Equal(_task.Name, taskResult.Name);
            Assert.Equal(_task.CategoryId, taskResult.CategoryId);
            Assert.Equal(_task.Examples, taskResult.Examples);
            Assert.Equal(_task.Description, taskResult.Description);
            Assert.Equal(_task.Points, taskResult.Points);
            Assert.Equal(_task.Public, taskResult.Public);
            Assert.Equal(_task.CheckerId, taskResult.CheckerId);
            Assert.Single(taskResult.Users);
            var user = taskResult.Users.First();
            Assert.Equal(_user.Id, user.UserId);

            Assert.Equal(1, _context.Tasks.Count());
            taskResult = _context.Tasks
                .Include(c => c.Users)
                .Single(c => c.Id == _task.Id);
            Assert.NotNull(taskResult);
            Assert.Equal(_task.Name, taskResult.Name);
            Assert.Equal(_task.CategoryId, taskResult.CategoryId);
            Assert.Equal(_task.Examples, taskResult.Examples);
            Assert.Equal(_task.Description, taskResult.Description);
            Assert.Equal(_task.Points, taskResult.Points);
            Assert.Equal(_task.Public, taskResult.Public);
            Assert.Equal(_task.CheckerId, taskResult.CheckerId);
            Assert.Single(taskResult.Users);
            user = taskResult.Users.First();
            Assert.Equal(_user.Id, user.UserId);
        }

        [Fact]
        public async Task CategoryNotPublic()
        {
            _category.Public = false;
            AddDataToDb();
            await _context.SaveChangesAsync();

            var result = await _controller.PostTask(_task);
            var badRequest = result as BadRequestObjectResult;
            Assert.NotNull(badRequest);

            var error = badRequest.Value as string;
            Assert.NotNull(error);
            Assert.Equal(nameof(_task.Public), error);

            Assert.Equal(0, _context.Tasks.Count());
        }

        [Fact]
        public async Task TaskInContestIsPublic()
        {
            _task.Public = false;
            _category.Contest = true;
            AddDataToDb();
            await _context.SaveChangesAsync();

            var result = await _controller.PostTask(_task);
            var createdAtResult = result as CreatedAtActionResult;
            Assert.NotNull(createdAtResult);

            var taskResult = createdAtResult.Value as TaskDefinition;
            Assert.NotNull(taskResult);
            Assert.True(taskResult.Public);

            taskResult = _context.Tasks
                .Include(c => c.Users)
                .Single(c => c.Id == _task.Id);
            Assert.True(taskResult.Public);
        }
    }
}
