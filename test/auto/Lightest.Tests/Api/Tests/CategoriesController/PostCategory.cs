using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Lightest.Data.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Moq;
using Xunit;

namespace Lightest.Tests.Api.Tests.CategoriesController
{
    public class PostCategory : BaseTest
    {
        protected override void AddDataToDb()
        {
            _parent.SubCategories = new List<Category>
            {
                _child1
            };
            _context.Tasks.Add(_task);
            _context.Categories.Add(_parent);
            _context.Categories.Add(_child1);
        }

        [Fact]
        public async Task Forbidden()
        {
            AddDataToDb();
            await _context.SaveChangesAsync();

            _accessServiceMock.Setup(m => m.CanAdd(It.IsAny<Category>(),
                It.Is<ApplicationUser>(u => u.Id == _user.Id)))
                .ReturnsAsync(false);

            var result = await _controller.PostCategory(_child2);

            Assert.IsAssignableFrom<ForbidResult>(result);
            Assert.Equal(2, _context.Categories.Count());
        }

        [Fact]
        public async Task Created()
        {
            AddDataToDb();
            await _context.SaveChangesAsync();

            var result = await _controller.PostCategory(_child2);
            var createdAtResult = result as CreatedAtActionResult;
            Assert.NotNull(createdAtResult);

            var categoryResult = createdAtResult.Value as Category;
            Assert.NotNull(categoryResult);
            Assert.Equal(_child2.Name, categoryResult.Name);
            Assert.Equal(_child2.Id, categoryResult.Id);
            Assert.Equal(_parent.Id, categoryResult.ParentId);
            Assert.Single(categoryResult.Users);
            var user = categoryResult.Users.First();
            Assert.Equal(_user.Id, user.UserId);

            Assert.Equal(3, _context.Categories.Count());
            categoryResult = _context.Categories
                .Include(c => c.Users)
                .Single(c => c.Id == _child2.Id);
            Assert.NotNull(categoryResult);
            Assert.Equal(_child2.Name, categoryResult.Name);
            Assert.Equal(_child2.Id, categoryResult.Id);
            Assert.Equal(_parent.Id, categoryResult.ParentId);
            Assert.Single(categoryResult.Users);
            user = categoryResult.Users.First();
            Assert.Equal(_user.Id, user.UserId);
        }

        [Fact]
        public async Task ParentNotPublic()
        {
            _child2.Public = true;
            _parent.Public = false;
            AddDataToDb();
            await _context.SaveChangesAsync();

            var result = await _controller.PostCategory(_child2);
            var badRequest = result as BadRequestObjectResult;
            Assert.NotNull(badRequest);

            var error = badRequest.Value as string;
            Assert.NotNull(error);
            Assert.Equal(nameof(_child2.ParentId), error);
        }

        [Fact]
        public async Task ParentNotFound()
        {
            _child2.Public = true;
            _child2.Parent = null;
            _child2.ParentId = Guid.NewGuid();
            AddDataToDb();
            await _context.SaveChangesAsync();

            var result = await _controller.PostCategory(_child2);
            var badRequest = result as BadRequestObjectResult;
            Assert.NotNull(badRequest);

            var error = badRequest.Value as string;
            Assert.NotNull(error);
            Assert.Equal(nameof(_child2.ParentId), error);
        }

        [Fact]
        public async Task ContestHasParent()
        {
            _child2.Contest = true;
            AddDataToDb();
            await _context.SaveChangesAsync();

            var result = await _controller.PostCategory(_child2);
            var badRequest = result as BadRequestObjectResult;
            Assert.NotNull(badRequest);
        }
    }
}
