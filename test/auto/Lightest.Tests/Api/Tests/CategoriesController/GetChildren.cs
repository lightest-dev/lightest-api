using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Lightest.Api.ResponseModels.CategoryViews;
using Lightest.Data.Models;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;

namespace Lightest.Tests.Api.Tests.CategoriesController
{
    public class GetChildren : BaseTest
    {
        [Fact]
        public async Task PublicAndAssigned()
        {
            AddDataToDb();
            await _context.SaveChangesAsync();

            var result = await _controller.GetChildren(_parent.Id);

            var okResult = result as OkObjectResult;
            Assert.NotNull(okResult);

            var categoriesResult = okResult.Value as CategoryChildrenView;

            Assert.NotNull(categoriesResult);
            Assert.Equal(2, categoriesResult.SubCategories.Count());

            Assert.Single(categoriesResult.Tasks);
        }

        [Fact]
        public async Task AssignedCategoryOnly()
        {
            _child1.Public = false;
            AddDataToDb();
            await _context.SaveChangesAsync();

            var result = await _controller.GetChildren(_parent.Id);

            var okResult = result as OkObjectResult;
            Assert.NotNull(okResult);

            var categoriesResult = okResult.Value as CategoryChildrenView;

            Assert.NotNull(categoriesResult);
            Assert.Single(categoriesResult.SubCategories);

            var subcategory = categoriesResult.SubCategories.First();
            Assert.Equal(subcategory.Id, _child2.Id);
        }

        [Fact]
        public async Task PublicCategoryOnly()
        {
            _child2.Users = new List<CategoryUser>();
            AddDataToDb();
            await _context.SaveChangesAsync();

            var result = await _controller.GetChildren(_parent.Id);

            var okResult = result as OkObjectResult;
            Assert.NotNull(okResult);

            var categoriesResult = okResult.Value as CategoryChildrenView;

            Assert.NotNull(categoriesResult);
            Assert.Single(categoriesResult.SubCategories);

            var subcategory = categoriesResult.SubCategories.First();
            Assert.Equal(subcategory.Id, _child1.Id);
        }

        [Fact]
        public async Task PublicTaskOnly()
        {
            _task.Users = new List<UserTask>();
            AddDataToDb();
            await _context.SaveChangesAsync();

            var result = await _controller.GetChildren(_parent.Id);

            var okResult = result as OkObjectResult;
            Assert.NotNull(okResult);

            var categoriesResult = okResult.Value as CategoryChildrenView;

            Assert.NotNull(categoriesResult);
            Assert.Single(categoriesResult.Tasks);
        }

        [Fact]
        public async Task AssignedTaskOnly()
        {
            _task.Public = false;
            AddDataToDb();
            await _context.SaveChangesAsync();

            var result = await _controller.GetChildren(_parent.Id);

            var okResult = result as OkObjectResult;
            Assert.NotNull(okResult);

            var categoriesResult = okResult.Value as CategoryChildrenView;

            Assert.NotNull(categoriesResult);
            Assert.Single(categoriesResult.Tasks);
        }

        [Fact]
        public async Task NoTaskAvailable()
        {
            _task.Public = false;
            _task.Users = new List<UserTask>();
            AddDataToDb();
            await _context.SaveChangesAsync();

            var result = await _controller.GetChildren(_parent.Id);

            var okResult = result as OkObjectResult;
            Assert.NotNull(okResult);

            var categoriesResult = okResult.Value as CategoryChildrenView;

            Assert.NotNull(categoriesResult);
            Assert.Empty(categoriesResult.Tasks);
        }

        [Fact]
        public async Task NoAccess()
        {
            AddDataToDb();
            await _context.SaveChangesAsync();

            _accessServiceMock.Setup(m => m.HasReadAccess(It.IsAny<Guid>(),
                It.Is<ApplicationUser>(u => u.Id == _user.Id)))
                .ReturnsAsync(false);

            var result = await _controller.GetChildren(_parent.Id);
            Assert.IsAssignableFrom<ForbidResult>(result);
        }

        [Fact]
        public async Task NotFound()
        {
            AddDataToDb();
            await _context.SaveChangesAsync();

            var result = await _controller.GetChildren(Guid.NewGuid());
            Assert.IsAssignableFrom<NotFoundResult>(result);
        }
    }
}
