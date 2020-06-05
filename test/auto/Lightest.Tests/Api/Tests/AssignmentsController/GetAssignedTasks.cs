using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Lightest.Api.ResponseModels.TaskViews;
using Lightest.Data.Models.TaskModels;
using Microsoft.AspNetCore.Mvc;
using Xunit;

namespace Lightest.Tests.Api.Tests.AssignmentsController
{
    public class GetAssignedTasks : BaseTest
    {
        private readonly TaskDefinition _secondTask;

        public GetAssignedTasks()
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
                Users = new List<Data.Models.Assignment>()
            };
        }

        protected override void AddDataToDb()
        {
            base.AddDataToDb();
            _context.Tasks.Add(_secondTask);
        }

        [Fact]
        public async Task HasAssignedTasks()
        {
            AddDataToDb();
            await _context.SaveChangesAsync();

            var result = _controller.GetAssignedTasks();

            var okResult = result as OkObjectResult;
            Assert.NotNull(okResult);

            var tasksResult = okResult.Value as IEnumerable<UserTaskView>;

            Assert.NotNull(tasksResult);
            Assert.Single(tasksResult);
        }

        [Fact]
        public async Task NoAssignedTasks()
        {
            _task.Users = new List<Data.Models.Assignment>();
            AddDataToDb();
            await _context.SaveChangesAsync();

            var result = _controller.GetAssignedTasks();

            var okResult = result as OkObjectResult;
            Assert.NotNull(okResult);

            var tasksResult = okResult.Value as IEnumerable<UserTaskView>;

            Assert.NotNull(tasksResult);
            Assert.Empty(tasksResult);
        }
    }
}
