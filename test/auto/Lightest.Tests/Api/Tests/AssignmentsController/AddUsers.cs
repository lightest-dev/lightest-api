using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Lightest.Data.Models;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;

namespace Lightest.Tests.Api.Tests.AssignmentsController
{
    public class AddUsers : BaseTest
    {
        private readonly Assignment _userTask;

        public AddUsers()
        {
            _userTask = new Assignment
            {
                User = _user,
                UserId = _user.Id,
                CanChangeAccess = false,
                CanRead = false,
                CanWrite = false,
                Deadline = DateTime.Now,
                IsOwner = false
            };
        }

        [Fact]
        public async Task Forbidden()
        {
            AddDataToDb();
            await _context.SaveChangesAsync();

            _accessServiceMock.Setup(m => m.CanEdit(It.IsAny<Guid>(),
                It.Is<ApplicationUser>(u => u.Id == _user.Id)))
                .ReturnsAsync(false);

            var result = await _controller.AddUsers(_task.Id, new[] { _userTask });

            Assert.IsAssignableFrom<ForbidResult>(result);
        }

        [Fact]
        public async Task NotFound()
        {
            AddDataToDb();
            await _context.SaveChangesAsync();

            var result = await _controller.AddUsers(Guid.NewGuid(), new[] { _userTask });

            Assert.IsAssignableFrom<NotFoundResult>(result);
        }

        [Fact]
        public async Task NoExistingUser()
        {
            _userTask.IsOwner = true;
            _task.Users = new List<Assignment>();
            AddDataToDb();
            await _context.SaveChangesAsync();

            var result = await _controller.AddUsers(_task.Id, new[] { _userTask });

            Assert.IsAssignableFrom<OkResult>(result);

            var task = _context.Tasks.First();
            Assert.Single(task.Users);

            var user = task.Users.First();
            Assert.Equal(_userTask.UserId, user.UserId);
            Assert.Equal(_userTask.CanChangeAccess, user.CanChangeAccess);
            Assert.Equal(_userTask.CanRead, user.CanRead);
            Assert.Equal(_userTask.CanWrite, user.CanWrite);
            Assert.Equal(_userTask.Deadline, user.Deadline);
            Assert.False(user.IsOwner);
        }

        [Fact]
        public async Task ExistingUser()
        {
            _task.Users.First().IsOwner = false;
            _userTask.IsOwner = true;
            AddDataToDb();
            await _context.SaveChangesAsync();

            var result = await _controller.AddUsers(_task.Id, new[] { _userTask });

            Assert.IsAssignableFrom<OkResult>(result);

            var task = _context.Tasks.First();
            Assert.Single(task.Users);

            var user = task.Users.First();
            Assert.Equal(_userTask.UserId, user.UserId);
            Assert.Equal(_userTask.CanChangeAccess, user.CanChangeAccess);
            Assert.Equal(_userTask.CanRead, user.CanRead);
            Assert.Equal(_userTask.CanWrite, user.CanWrite);
            Assert.Equal(_userTask.Deadline, user.Deadline);
            Assert.False(user.IsOwner);
        }

        [Fact]
        public async Task ExistingOwner()
        {
            var originalUser = _task.Users.First();
            originalUser.IsOwner = true;
            AddDataToDb();
            await _context.SaveChangesAsync();

            var result = await _controller.AddUsers(_task.Id, new[] { _userTask });

            Assert.IsAssignableFrom<OkResult>(result);

            var task = _context.Tasks.First();
            Assert.Single(task.Users);

            var user = task.Users.First();
            Assert.Equal(originalUser.UserId, user.UserId);
            Assert.Equal(originalUser.CanChangeAccess, user.CanChangeAccess);
            Assert.Equal(originalUser.CanRead, user.CanRead);
            Assert.Equal(originalUser.CanWrite, user.CanWrite);
            Assert.Equal(originalUser.Deadline, user.Deadline);
            Assert.True(user.IsOwner);
        }
    }
}
