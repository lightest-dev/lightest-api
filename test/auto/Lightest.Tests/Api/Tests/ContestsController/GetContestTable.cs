using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Lightest.Data.Models;
using Lightest.Data.Models.TaskModels;
using Microsoft.AspNetCore.Mvc;
using Xunit;

namespace Lightest.Tests.Api.Tests.ContestsController
{
    public class GetContestTable : BaseTest
    {
        private TaskDefinition EmptyTaskDefinition { get; set; }

        public GetContestTable()
        {
            _category.Name = "name";

            _task.Users = new List<UserTask>
            {
                new UserTask
                {
                    UserId = Guid.NewGuid().ToString(),
                    HighScore = 0
                },
                new UserTask
                {
                    UserId = Guid.NewGuid().ToString(),
                    HighScore = 60
                }
            };
            EmptyTaskDefinition = new TaskDefinition
            {
                Id = Guid.NewGuid(),
                CategoryId = _category.Id
            };
        }

        [Fact]
        public async Task NotFound()
        {
            var result = await _controller.GetContestTable(Guid.NewGuid());

            Assert.IsAssignableFrom<NotFoundResult>(result.Result);
        }

        [Fact]
        public async Task AllTasksReturned()
        {
            AddDataToDb();
            _context.Tasks.Add(EmptyTaskDefinition);
            await _context.SaveChangesAsync();

            var result = await _controller.GetContestTable(_category.Id);

            Assert.Equal(2, result.Value.TaskResults.Count());
            Assert.Contains(result.Value.TaskResults, r => r.TaskId == _task.Id);
            Assert.Contains(result.Value.TaskResults, r => r.TaskId == EmptyTaskDefinition.Id);
        }

        [Fact]
        public async Task AllUsersReturned()
        {
            AddDataToDb();
            await _context.SaveChangesAsync();

            var result = await _controller.GetContestTable(_category.Id);

            var firstUser = _task.Users.First();
            var secondUser = _task.Users.Skip(1).First();

            var task = result.Value.TaskResults.Single();
            Assert.Contains(task.UserResults, u => u.UserId == firstUser.UserId && u.Score == firstUser.HighScore);
            Assert.Contains(task.UserResults, u => u.UserId == secondUser.UserId && u.Score == secondUser.HighScore);
        }
    }
}
