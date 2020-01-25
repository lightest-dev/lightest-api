using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Lightest.Data.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Moq;
using Xunit;

namespace Lightest.Tests.Api.Tests.GroupsController
{
    public class PostGroup : BaseTest
    {
        protected override void AddDataToDb()
        {
            _parent.SubGroups = new List<Group>
            {
                _child1
            };
            _context.Groups.Add(_parent);
            _context.Groups.Add(_child1);
        }

        [Fact]
        public async Task Forbidden()
        {
            AddDataToDb();
            await _context.SaveChangesAsync();

            _accessServiceMock.Setup(m => m.CanEdit(It.IsAny<Guid>(),
                It.Is<ApplicationUser>(u => u.Id == _user.Id)))
                .ReturnsAsync(false);

            var result = await _controller.PostGroup(_child2);

            Assert.IsAssignableFrom<ForbidResult>(result);
            Assert.Equal(2, _context.Groups.Count());
        }

        [Fact]
        public async Task Created()
        {
            AddDataToDb();
            await _context.SaveChangesAsync();

            var result = await _controller.PostGroup(_child2);
            var createdAtResult = result as CreatedAtActionResult;
            Assert.NotNull(createdAtResult);

            var groupResult = createdAtResult.Value as Group;
            Assert.NotNull(groupResult);
            Assert.Equal(_child2.Name, groupResult.Name);
            Assert.Equal(_child2.Id, groupResult.Id);
            Assert.Equal(_parent.Id, groupResult.ParentId);
            Assert.Single(groupResult.Users);
            var user = groupResult.Users.First();
            Assert.Equal(_user.Id, user.UserId);

            Assert.Equal(3, _context.Groups.Count());
            groupResult = _context.Groups
                .Include(c => c.Users)
                .Single(c => c.Id == _child2.Id);
            Assert.NotNull(groupResult);
            Assert.Equal(_child2.Name, groupResult.Name);
            Assert.Equal(_child2.Id, groupResult.Id);
            Assert.Equal(_parent.Id, groupResult.ParentId);
            Assert.Single(groupResult.Users);
            user = groupResult.Users.First();
            Assert.Equal(_user.Id, user.UserId);
        }

        [Fact]
        public async Task ParentNotFound()
        {
            _child2.Parent = null;
            _child2.ParentId = Guid.NewGuid();
            AddDataToDb();
            await _context.SaveChangesAsync();

            var result = await _controller.PostGroup(_child2);
            var badRequest = result as BadRequestObjectResult;
            Assert.NotNull(badRequest);

            var error = badRequest.Value as string;
            Assert.NotNull(error);
            Assert.Equal(nameof(_child2.ParentId), error);
        }
    }
}
