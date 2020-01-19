using System;
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
                ParentId = null
            };
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
        }

        [Fact]
        public async Task Forbidden()
        {
            AddDataToDb();
            await _context.SaveChangesAsync();

            _accessServiceMock.Setup(m => m.CanWrite(It.IsAny<Guid>(),
                It.Is<ApplicationUser>(u => u.Id == _user.Id)))
                .ReturnsAsync(false);

            var result = await _controller.PutCategory(_updatedChild.Id, _updatedChild);

            Assert.IsAssignableFrom<ForbidResult>(result);
            Assert.Equal(3, _context.Categories.Count());
            var categoryResult = _context.Categories
                .Single(c => c.Id == _child2.Id);

            Assert.Equal(_child2.Name, categoryResult.Name);
            Assert.Equal(_child2.ParentId, categoryResult.ParentId);
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
        }
    }
}
