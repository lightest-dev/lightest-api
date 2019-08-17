using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Lightest.Api.ResponseModels;
using Lightest.Data.Models;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;

namespace Lightest.Tests.Api.Tests.CheckersController
{
    public class GetCheckers : BaseTest
    {
        [Fact]
        public async Task NoCheckers()
        {
            var result = await _controller.GetCheckers();

            var okResult = result as OkObjectResult;
            Assert.NotNull(okResult);

            var checkersResult = okResult.Value as IEnumerable<BaseChecker>;

            Assert.NotNull(checkersResult);
            Assert.Empty(checkersResult);
        }

        [Fact]
        public async Task Forbidden()
        {
            _context.Checkers.Add(_checker);
            await _context.SaveChangesAsync();

            _accessServiceMock.Setup(m => m.CheckReadAccess(It.IsAny<Checker>(),
                It.Is<ApplicationUser>(u => u.Id == _user.Id)))
                .Returns(false);

            var result = await _controller.GetCheckers();

            Assert.IsAssignableFrom<ForbidResult>(result);
        }

        [Fact]
        public async Task MultipleCheckers()
        {
            _context.Checkers.Add(_checker);
            var secondChecker = new Checker
            {
                Id = Guid.NewGuid(),
                Code = "code2",
                Name = "name2"
            };
            _context.Checkers.Add(secondChecker);
            await _context.SaveChangesAsync();

            var result = await _controller.GetCheckers();

            var okResult = result as OkObjectResult;
            Assert.NotNull(okResult);

            var checkersResult = okResult.Value as IEnumerable<BaseChecker>;

            Assert.NotNull(checkersResult);
            Assert.Equal(2, checkersResult.Count());
            Assert.All(checkersResult, (checker) =>
            {
                Assert.Null(checker.Compiled);
                Assert.True(checker.Id == _checker.Id || checker.Id == secondChecker.Id);
                Assert.True(checker.Name == _checker.Name || checker.Name == secondChecker.Name);
            });
        }
    }
}
