using System;
using System.Linq;
using System.Threading.Tasks;
using Lightest.Data.Models;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;

namespace Lightest.Tests.Api.Tests.CategoriesController
{
    public class DeleteCategory : BaseTest
    {
        [Fact]
        public async Task EntryNotFound()
        {
            AddDataToDb();
            await _context.SaveChangesAsync();

            var result = await _controller.DeleteCategory(Guid.NewGuid());

            Assert.IsAssignableFrom<NotFoundResult>(result);
            Assert.Equal(3, _context.Categories.Count());
        }

        [Fact]
        public async Task Forbidden()
        {
            AddDataToDb();
            await _context.SaveChangesAsync();

            _accessServiceMock.Setup(m => m.CanWrite(It.IsAny<Guid>(),
                It.Is<ApplicationUser>(u => u.Id == _user.Id)))
                .ReturnsAsync(false);

            var result = await _controller.DeleteCategory(_child2.Id);

            Assert.IsAssignableFrom<ForbidResult>(result);
            Assert.Equal(3, _context.Categories.Count());
        }

        [Fact]
        public async Task Deleted()
        {
            AddDataToDb();
            await _context.SaveChangesAsync();

            var result = await _controller.DeleteCategory(_child2.Id);

            var okResult = result as OkObjectResult;
            Assert.NotNull(okResult);

            var categoryResult = okResult.Value as Category;
            Assert.NotNull(categoryResult);

            Assert.Equal(_child2.Name, categoryResult.Name);
            Assert.Equal(_child2.ParentId, categoryResult.ParentId);
            Assert.Equal(_child2.Id, categoryResult.Id);
            Assert.Equal(_child2.Public, categoryResult.Public);

            Assert.Equal(2, _context.Categories.Count());
        }
    }
}
