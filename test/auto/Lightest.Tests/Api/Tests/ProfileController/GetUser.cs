using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Lightest.Api.ResponseModels.UserViews;
using Lightest.Data.Models;
using Lightest.Data.Models.TaskModels;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;

namespace Lightest.Tests.Api.Tests.ProfileController
{
    public class GetUser : BaseTest
    {
        [Fact]
        public async Task Forbidden()
        {
            _context.Users.Add(_user);
            await _context.SaveChangesAsync();

            _accessServiceMock.Setup(m => m.HasReadAccess(It.IsAny<ApplicationUser>(),
                It.Is<ApplicationUser>(u => u.Id == _user.Id)))
                .Returns(false);

            var result = await _controller.GetUser(_user.Id);

            Assert.IsAssignableFrom<ForbidResult>(result);
        }

        [Fact]
        public async Task UserReturned()
        {
            var task = new TaskDefinition
            {
                Id = Guid.NewGuid(),
                Name = "task"
            };
            var group = new Group
            {
                Id = Guid.NewGuid(),
                Name = "group"
            };
            _user.Groups = new List<UserGroup>
            {
                new UserGroup
                {
                    UserId = _user.Id,
                    User = _user,
                    Group = group,
                    GroupId = group.Id
                }
            };
            var userTask = new UserTask
            {
                Completed = true,
                HighScore = 5,
                Task = task,
                TaskId = task.Id
            };
            _user.Tasks = new List<UserTask>
            {
                userTask
            };
            _context.Users.Add(_user);
            await _context.SaveChangesAsync();

            var result = await _controller.GetUser(_user.Id);
            var okResult = result as OkObjectResult;
            Assert.NotNull(okResult);

            var user = okResult.Value as CompleteUserView;
            Assert.NotNull(user);
            Assert.Equal(_user.Name, user.Name);
            Assert.Equal(_user.Surname, user.Surname);
            Assert.Equal(_user.Email, user.Email);
            Assert.Equal(_user.UserName, user.Login);

            Assert.Single(user.Groups);
            var resultGroup = user.Groups.First();
            Assert.Equal(group.Id, resultGroup.Id);
            Assert.Equal(group.Name, resultGroup.Name);
        }

        [Fact]
        public async Task NotFound()
        {
            var result = await _controller.GetUser(Guid.NewGuid().ToString());
            Assert.IsAssignableFrom<NotFoundResult>(result);
        }
    }
}
