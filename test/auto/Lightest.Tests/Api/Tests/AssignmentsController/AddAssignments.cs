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
    public class AddAssignments : BaseTest
    {
        private readonly AddOrUpdateAssignmentsRequest _request;
        private readonly AssignmentRequest _assignmentRequest;

        public AddAssignments()
        {
            _assignmentRequest = new AssignmentRequest
            {
                UserId = _user.Id,
                CanChangeAccess = false,
                CanRead = false,
                CanWrite = false,
                Deadline = DateTime.Now,
                IsOwner = false
            };

            _request = new AddOrUpdateAssignmentsRequest
            {
                TaskId = _task.Id,
                Assignments = new AssignmentRequest[]
                {
                    _assignmentRequest
                }
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

            var result = await _controller.AddAssignments(_task.Id, _request);

            Assert.IsAssignableFrom<ForbidResult>(result);
        }

        [Fact]
        public async Task IdsDoNotMatch()
        {
            AddDataToDb();
            await _context.SaveChangesAsync();

            var result = await _controller.AddAssignments(Guid.NewGuid(), _request);

            Assert.IsAssignableFrom<BadRequestObjectResult>(result);
        }

        [Fact]
        public async Task NotFound()
        {
            AddDataToDb();
            await _context.SaveChangesAsync();

            var newId = Guid.NewGuid();
            _request.TaskId = newId;

            var result = await _controller.AddAssignments(newId, _request);

            Assert.IsAssignableFrom<NotFoundResult>(result);
        }

        [Fact]
        public async Task NoExistingUser()
        {
            _assignmentRequest.IsOwner = true;
            _task.Users = new List<Assignment>();
            AddDataToDb();
            await _context.SaveChangesAsync();

            var result = await _controller.AddAssignments(_task.Id, _request);

            Assert.IsAssignableFrom<OkResult>(result);

            var task = _context.Tasks.First();
            Assert.Single(task.Users);

            var user = task.Users.First();
            Assert.Equal(_assignmentRequest.UserId, user.UserId);
            Assert.Equal(_assignmentRequest.CanChangeAccess, user.CanChangeAccess);
            Assert.Equal(_assignmentRequest.CanRead, user.CanRead);
            Assert.Equal(_assignmentRequest.CanWrite, user.CanWrite);
            Assert.Equal(_assignmentRequest.Deadline, user.Deadline);
            Assert.False(user.IsOwner);
        }

        [Fact]
        public async Task ExistingUser()
        {
            _task.Users.First().IsOwner = false;
            _assignmentRequest.IsOwner = true;
            AddDataToDb();
            await _context.SaveChangesAsync();

            var result = await _controller.AddAssignments(_task.Id, _request);

            Assert.IsAssignableFrom<OkResult>(result);

            var task = _context.Tasks.First();
            Assert.Single(task.Users);

            var user = task.Users.First();
            Assert.Equal(_assignmentRequest.UserId, user.UserId);
            Assert.Equal(_assignmentRequest.CanChangeAccess, user.CanChangeAccess);
            Assert.Equal(_assignmentRequest.CanRead, user.CanRead);
            Assert.Equal(_assignmentRequest.CanWrite, user.CanWrite);
            Assert.Equal(_assignmentRequest.Deadline, user.Deadline);
            Assert.False(user.IsOwner);
        }

        [Fact]
        public async Task ExistingOwner()
        {
            var originalUser = _task.Users.First();
            originalUser.IsOwner = true;
            AddDataToDb();
            await _context.SaveChangesAsync();

            var result = await _controller.AddAssignments(_task.Id, _request);

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
