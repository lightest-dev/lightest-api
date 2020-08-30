using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Lightest.Data.Models;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;

namespace Lightest.Tests.Api.Tests.CategoriesController
{
    public class PutCategory : BaseTest
    {
        protected readonly Category _updatedChild;

        public PutCategory()
        {
            _updatedChild = new Category
            {
                Id = _child2.Id,
                Name = "updatedName",
                ParentId = null,
                Public = true
            };
        }

        protected override void AddDataToDb()
        {
            _parent.SubCategories = new List<Category>
            {
                _child1,
                _child2
            };
            _context.Tasks.Add(_task);
            _context.Categories.Add(_parent);
            _context.Categories.Add(_child1);
            _context.Categories.Add(_child2);
        }

        [Fact]
        public async Task IdsDontMatch()
        {
            AddDataToDb();
            await _context.SaveChangesAsync();

            var result = await _controller.PutCategory(Guid.NewGuid(), _updatedChild);

            Assert.IsAssignableFrom<BadRequestResult>(result);
            Assert.Equal(3, _context.Categories.Count());
            var categoryResult = _context.Categories
                .Single(c => c.Id == _child2.Id);

            Assert.Equal(_child2.Name, categoryResult.Name);
            Assert.Equal(_child2.ParentId, categoryResult.ParentId);
            Assert.Equal(_child2.Public, categoryResult.Public);
        }

        [Fact]
        public async Task EntryNotFound()
        {
            _updatedChild.Id = Guid.NewGuid();
            AddDataToDb();
            await _context.SaveChangesAsync();

            var result = await _controller.PutCategory(_updatedChild.Id, _updatedChild);

            Assert.IsAssignableFrom<NotFoundResult>(result);
            Assert.Equal(3, _context.Categories.Count());
            var categoryResult = _context.Categories
                .Single(c => c.Id == _child2.Id);

            Assert.Equal(_child2.Name, categoryResult.Name);
            Assert.Equal(_child2.ParentId, categoryResult.ParentId);
            Assert.Equal(_child2.Public, categoryResult.Public);
        }

        [Fact]
        public async Task Forbidden()
        {
            AddDataToDb();
            await _context.SaveChangesAsync();

            _accessServiceMock.Setup(m => m.CanEdit(It.IsAny<Guid>(),
                It.Is<ApplicationUser>(u => u.Id == _user.Id)))
                .ReturnsAsync(false);

            var result = await _controller.PutCategory(_updatedChild.Id, _updatedChild);

            Assert.IsAssignableFrom<ForbidResult>(result);
            Assert.Equal(3, _context.Categories.Count());
            var categoryResult = _context.Categories
                .Single(c => c.Id == _child2.Id);

            Assert.Equal(_child2.Name, categoryResult.Name);
            Assert.Equal(_child2.ParentId, categoryResult.ParentId);
            Assert.Equal(_child2.Public, categoryResult.Public);
        }

        [Fact]
        public async Task Updated()
        {
            AddDataToDb();
            await _context.SaveChangesAsync();

            var result = await _controller.PutCategory(_updatedChild.Id, _updatedChild);

            Assert.IsAssignableFrom<OkResult>(result);
            Assert.Equal(3, _context.Categories.Count());
            var categoryResult = _context.Categories
                .Single(c => c.Id == _child2.Id);

            Assert.Equal(_updatedChild.Name, categoryResult.Name);
            Assert.Equal(_updatedChild.ParentId, categoryResult.ParentId);
            Assert.Equal(_updatedChild.Public, categoryResult.Public);
        }

        [Fact]
        public async Task ParentNotPublic()
        {
            _child2.Public = true;
            _parent.Public = false;
            AddDataToDb();
            await _context.SaveChangesAsync();

            var result = await _controller.PutCategory(_child2.Id, _child2);
            var badRequest = result as BadRequestObjectResult;
            Assert.NotNull(badRequest);

            var error = badRequest.Value as string;
            Assert.NotNull(error);
            Assert.Equal(nameof(_child2.ParentId), error);

            var categoryResult = _context.Categories
                .Single(c => c.Id == _child2.Id);

            Assert.Equal(_child2.Name, categoryResult.Name);
            Assert.Equal(_child2.ParentId, categoryResult.ParentId);
            Assert.Equal(_child2.Public, categoryResult.Public);
        }
    }
}
