using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Lightest.Api.RequestModels.AssignmentRequests;
using Lightest.Data.Models;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;

namespace Lightest.Tests.Api.Tests.AssignmentsController
{
    public class AddGroupAssignments : BaseTest
    {
        private readonly AssignGroupRequest _request;
        private readonly Group _group;

        public AddGroupAssignments()
        {
            _group = new Group
            {
                Id = Guid.NewGuid()
            };
            _group.Users = new List<UserGroup>
            {
                new UserGroup
                {
                    UserId = _user.Id,
                    GroupId = _group.Id,
                }
            };

            _request = new AssignGroupRequest
            {
                GroupId = _group.Id,
                TaskId = _task.Id,
                Deadline = DateTime.Now
            };
        }

        protected override void AddDataToDb()
        {
            base.AddDataToDb();
            _context.Groups.Add(_group);
        }

        [Fact]
        public async Task Forbidden()
        {
            AddDataToDb();
            await _context.SaveChangesAsync();

            _accessServiceMock.Setup(m => m.CanEdit(It.IsAny<Guid>(),
                It.Is<ApplicationUser>(u => u.Id == _user.Id)))
                .ReturnsAsync(false);

            var result = await _controller.AddGroupAssignments(_task.Id, _request);

            Assert.IsAssignableFrom<ForbidResult>(result);
        }

        [Fact]
        public async Task IdsDoNotMatch()
        {
            AddDataToDb();
            await _context.SaveChangesAsync();

            var result = await _controller.AddGroupAssignments(Guid.NewGuid(), _request);

            Assert.IsAssignableFrom<BadRequestObjectResult>(result);
        }

        [Fact]
        public async Task TaskNotFound()
        {
            AddDataToDb();
            await _context.SaveChangesAsync();

            var newId = Guid.NewGuid();
            _request.TaskId = newId;

            var result = await _controller.AddGroupAssignments(newId, _request);

            Assert.IsAssignableFrom<NotFoundResult>(result);
        }

        [Fact]
        public async Task NoExistingUser()
        {
            _task.Users = new List<Assignment>();
            AddDataToDb();
            await _context.SaveChangesAsync();

            var result = await _controller.AddGroupAssignments(_task.Id, _request);

            Assert.IsAssignableFrom<OkResult>(result);

            var task = _context.Tasks.First();
            Assert.Single(task.Users);

            var user = task.Users.First();
            Assert.Equal(_user.Id, user.UserId);
            Assert.False(user.CanChangeAccess);
            Assert.True(user.CanRead);
            Assert.False(user.CanWrite);
            Assert.Equal(_request.Deadline, user.Deadline);
            Assert.False(user.IsOwner);
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public async Task ExistingUser(bool isOwner)
        {
            var oldDate = DateTime.Now.AddDays(-1);
            var existingAssignment = _task.Users.First();
            existingAssignment.IsOwner = isOwner;
            existingAssignment.CanRead = true;
            existingAssignment.CanWrite = true;
            existingAssignment.CanChangeAccess = true;
            existingAssignment.Deadline = oldDate;
            AddDataToDb();
            await _context.SaveChangesAsync();

            var result = await _controller.AddGroupAssignments(_task.Id, _request);

            Assert.IsAssignableFrom<OkResult>(result);

            var task = _context.Tasks.First();
            Assert.Single(task.Users);

            var newAssignment = task.Users.First();
            Assert.Equal(_user.Id, newAssignment.UserId);
            Assert.True(newAssignment.CanChangeAccess);
            Assert.True(newAssignment.CanRead);
            Assert.True(newAssignment.CanWrite);
            Assert.Equal(oldDate, newAssignment.Deadline);
            Assert.Equal(isOwner, newAssignment.IsOwner);
        }
    }
}
