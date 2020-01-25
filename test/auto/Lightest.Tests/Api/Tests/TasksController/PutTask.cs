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
    public class PutTask : BaseTest
    {
        protected readonly TaskDefinition _updatedTask;
        protected readonly Category _newCategory;

        public PutTask()
        {
            _newCategory = new Category
            {
                Id = Guid.NewGuid(),
                Name = "New Name"
            };

            _task.Public = true;

            _updatedTask = new TaskDefinition
            {
                Id = _task.Id,
                CategoryId = _newCategory.Id,
                Examples = "new examples",
                Description = "new description",
                Points = 10,
                Public = false,
                CheckerId = Guid.NewGuid()
            };
        }

        [Fact]
        public async Task IdsDontMatch()
        {
            AddDataToDb();
            await _context.SaveChangesAsync();

            var result = await _controller.PutTask(Guid.NewGuid(), _updatedTask);

            Assert.IsAssignableFrom<BadRequestResult>(result);
            Assert.Equal(1, _context.Tasks.Count());
            var taskResult = _context.Tasks
                .Single(c => c.Id == _task.Id);

            Assert.Equal(_task.Name, taskResult.Name);
            Assert.Equal(_task.CategoryId, taskResult.CategoryId);
            Assert.Equal(_task.Examples, taskResult.Examples);
            Assert.Equal(_task.Description, taskResult.Description);
            Assert.Equal(_task.Points, taskResult.Points);
            Assert.Equal(_task.Public, taskResult.Public);
            Assert.Equal(_task.CheckerId, taskResult.CheckerId);
        }

        [Fact]
        public async Task EntryNotFound()
        {
            _updatedTask.Id = Guid.NewGuid();
            AddDataToDb();
            await _context.SaveChangesAsync();

            var result = await _controller.PutTask(_updatedTask.Id, _updatedTask);

            Assert.IsAssignableFrom<NotFoundResult>(result);
            Assert.Equal(1, _context.Tasks.Count());
            var taskResult = _context.Tasks
                .Single(c => c.Id == _task.Id);

            Assert.Equal(_task.Name, taskResult.Name);
            Assert.Equal(_task.CategoryId, taskResult.CategoryId);
            Assert.Equal(_task.Examples, taskResult.Examples);
            Assert.Equal(_task.Description, taskResult.Description);
            Assert.Equal(_task.Points, taskResult.Points);
            Assert.Equal(_task.Public, taskResult.Public);
            Assert.Equal(_task.CheckerId, taskResult.CheckerId);
        }

        [Fact]
        public async Task Forbidden()
        {
            AddDataToDb();
            await _context.SaveChangesAsync();

            _accessServiceMock.Setup(m => m.CanEdit(It.IsAny<Guid>(),
                It.Is<ApplicationUser>(u => u.Id == _user.Id)))
                .ReturnsAsync(false);

            var result = await _controller.PutTask(_updatedTask.Id, _updatedTask);

            Assert.IsAssignableFrom<ForbidResult>(result);
            Assert.Equal(1, _context.Tasks.Count());
            var taskResult = _context.Tasks
                .Single(c => c.Id == _task.Id);

            Assert.Equal(_task.Name, taskResult.Name);
            Assert.Equal(_task.CategoryId, taskResult.CategoryId);
            Assert.Equal(_task.Examples, taskResult.Examples);
            Assert.Equal(_task.Description, taskResult.Description);
            Assert.Equal(_task.Points, taskResult.Points);
            Assert.Equal(_task.Public, taskResult.Public);
            Assert.Equal(_task.CheckerId, taskResult.CheckerId);
        }

        [Fact]
        public async Task Updated()
        {
            _context.Categories.Add(_newCategory);
            AddDataToDb();
            await _context.SaveChangesAsync();

            var result = await _controller.PutTask(_updatedTask.Id, _updatedTask);

            Assert.IsAssignableFrom<OkResult>(result);
            Assert.Equal(1, _context.Tasks.Count());
            var taskResult = _context.Tasks
                .Single(c => c.Id == _task.Id);

            Assert.Equal(_updatedTask.CategoryId, taskResult.CategoryId);
            Assert.Equal(_updatedTask.Examples, taskResult.Examples);
            Assert.Equal(_updatedTask.Description, taskResult.Description);
            Assert.Equal(_updatedTask.Points, taskResult.Points);
            Assert.Equal(_updatedTask.Public, taskResult.Public);
            Assert.Equal(_updatedTask.CheckerId, taskResult.CheckerId);
        }

        [Fact]
        public async Task PublicCantBeModified()
        {
            _newCategory.Contest = true;
            _context.Categories.Add(_newCategory);
            AddDataToDb();
            await _context.SaveChangesAsync();

            var result = await _controller.PutTask(_updatedTask.Id, _updatedTask);

            Assert.IsAssignableFrom<BadRequestObjectResult>(result);
        }
    }
}
