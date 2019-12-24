using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Lightest.Api.ResponseModels.CategoryViews;
using Lightest.Data.Models;
using Microsoft.AspNetCore.Mvc;
using Xunit;

namespace Lightest.Tests.Api.Tests.CategoriesController
{
    public class GetAvailableCategories : BaseTest
    {
        [Fact]
        public async Task HasAssignedCategory()
        {
            _parent.Public = false;
            var parentUser = new CategoryUser
            {
                UserId = _user.Id,
                CategoryId = _parent.Id,
                User = _user,
                Category = _parent,
                CanRead = false,
                CanWrite = true,
                CanChangeAccess = true
            };
            _parent.Users = new List<CategoryUser>
            {
                parentUser
            };

            AddDataToDb();
            await _context.SaveChangesAsync();

            var result = await _controller.GetAvailableCategories(new Sieve.Models.SieveModel());

            var okResult = result as OkObjectResult;
            Assert.NotNull(okResult);

            var categoriesResult = okResult.Value as IEnumerable<ListCategoryView>;

            Assert.NotNull(categoriesResult);

            var group = categoriesResult.Single();
            Assert.Equal(_parent.Name, group.Name);
            Assert.Equal(_parent.Id, group.Id);
            Assert.Equal(parentUser.CanRead, group.CanRead);
            Assert.Equal(parentUser.CanWrite, group.CanWrite);
            Assert.Equal(parentUser.CanChangeAccess, group.CanChangeAccess);
        }

        [Fact]
        public async Task HasPublicCategory()
        {
            _parent.Public = true;
            AddDataToDb();
            await _context.SaveChangesAsync();

            var result = await _controller.GetAvailableCategories(new Sieve.Models.SieveModel());

            var okResult = result as OkObjectResult;
            Assert.NotNull(okResult);

            var categoriesResult = okResult.Value as IEnumerable<ListCategoryView>;

            Assert.NotNull(categoriesResult);

            var group = categoriesResult.Single();
            Assert.Equal(_parent.Name, group.Name);
            Assert.Equal(_parent.Id, group.Id);
            Assert.True(group.CanRead);
            Assert.False(group.CanWrite);
            Assert.False(group.CanChangeAccess);
        }

        [Fact]
        public async Task NoAssignedCategory()
        {
            _parent.Public = false;
            AddDataToDb();
            await _context.SaveChangesAsync();

            var result = await _controller.GetAvailableCategories(new Sieve.Models.SieveModel());

            var okResult = result as OkObjectResult;
            Assert.NotNull(okResult);

            var categoriesResult = okResult.Value as IEnumerable<ListCategoryView>;

            Assert.NotNull(categoriesResult);
            Assert.Empty(categoriesResult);
        }
    }
}
