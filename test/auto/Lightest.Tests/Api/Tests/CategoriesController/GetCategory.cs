using System;
using System.Linq;
using System.Threading.Tasks;
using Lightest.Api.ResponseModels;
using Lightest.Data.Models;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;

namespace Lightest.Tests.Api.Tests.CategoriesController
{
    public class GetCategory : BaseTest
    {
        [Fact]
        public async Task NoAccess()
        {
            AddDataToDb();
            await _context.SaveChangesAsync();

            _accessServiceMock.Setup(m => m.CheckWriteAccess(It.IsAny<Category>(),
                It.Is<ApplicationUser>(u => u.Id == _user.Id)))
                .Returns(false);

            var result = await _controller.GetCategory(_parent.Id);
            Assert.IsAssignableFrom<ForbidResult>(result);
        }

        [Fact]
        public async Task NotFound()
        {
            AddDataToDb();
            await _context.SaveChangesAsync();

            var result = await _controller.GetCategory(Guid.NewGuid());
            Assert.IsAssignableFrom<NotFoundResult>(result);
        }

        [Fact]
        public async Task ParentCategoryFound()
        {
            AddDataToDb();
            await _context.SaveChangesAsync();

            var result = await _controller.GetCategory(_parent.Id);

            var okResult = result as OkObjectResult;
            Assert.NotNull(okResult);

            var categoriesResult = okResult.Value as CompleteCategory;

            Assert.NotNull(categoriesResult);
            Assert.Equal(_parent.Id, categoriesResult.Id);
            Assert.Equal(_parent.Name, categoriesResult.Name);
            Assert.Equal(2, categoriesResult.SubCategories.Count());

            Assert.Single(categoriesResult.Tasks);
            var task = categoriesResult.Tasks.First();
            Assert.Equal(_task.Id, task.Id);
            Assert.Equal(_task.Name, task.Name);
        }

        [Fact]
        public async Task ChildCategoryFound()
        {
            AddDataToDb();
            await _context.SaveChangesAsync();

            var result = await _controller.GetCategory(_child2.Id);

            var okResult = result as OkObjectResult;
            Assert.NotNull(okResult);

            var categoriesResult = okResult.Value as CompleteCategory;

            Assert.NotNull(categoriesResult);
            Assert.Equal(_child2.Id, categoriesResult.Id);
            Assert.Equal(_child2.Name, categoriesResult.Name);
            Assert.Empty(categoriesResult.SubCategories);

            Assert.Single(categoriesResult.Users);
            var user = categoriesResult.Users.First();
            Assert.Equal(_user.Id, user.Id);
            Assert.Equal(_user.UserName, user.UserName);
        }
    }
}
