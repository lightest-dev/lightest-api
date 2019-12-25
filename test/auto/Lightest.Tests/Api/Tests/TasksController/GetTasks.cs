using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Lightest.Data.Models;
using Lightest.Data.Models.TaskModels;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;

namespace Lightest.Tests.Api.Tests.TasksController
{
    public class GetTasks : BaseTest
    {
        private readonly TaskDefinition _secondTask;

        public GetTasks()
        {
            _secondTask = new TaskDefinition
            {
                Id = Guid.NewGuid(),
                Public = true,
                Points = 100,
                Checker = _checker,
                CheckerId = _checker.Id,
                Category = _category,
                CategoryId = _category.Id,
                Name = "name",
                Tests = new List<Test>(),
                Languages = new List<TaskLanguage>(),
                Users = new List<UserTask>()
            };
        }

        protected override void AddDataToDb()
        {
            base.AddDataToDb();
            _context.Tasks.Add(_secondTask);
        }

        [Fact]
        public async Task HasAdminAccess()
        {
            AddDataToDb();
            await _context.SaveChangesAsync();

            var result = await _controller.GetTasks(new Sieve.Models.SieveModel());

            var okResult = result as OkObjectResult;
            Assert.NotNull(okResult);

            var tasksResult = okResult.Value as IEnumerable<TaskDefinition>;

            Assert.NotNull(tasksResult);
            Assert.Equal(2, tasksResult.Count());
        }

        [Fact]
        public async Task NoAdminAccess()
        {
            AddDataToDb();
            await _context.SaveChangesAsync();

            _accessServiceMock.Setup(m => m.HasAdminAccess(It.Is<ApplicationUser>(u => u.Id == _user.Id)))
                .Returns(false);

            var result = await _controller.GetTasks(new Sieve.Models.SieveModel());
            Assert.IsAssignableFrom<ForbidResult>(result);
        }
    }
}
