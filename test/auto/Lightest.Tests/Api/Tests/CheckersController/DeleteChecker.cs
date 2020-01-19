using System;
using System.Linq;
using System.Threading.Tasks;
using Lightest.Data.Models;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;

namespace Lightest.Tests.Api.Tests.CheckersController
{
    public class DeleteChecker : BaseTest
    {
        [Fact]
        public async Task NoTestFound()
        {
            var result = await _controller.DeleteChecker(Guid.NewGuid());

            Assert.IsAssignableFrom<NotFoundResult>(result);
        }

        [Fact]
        public async Task Forbidden()
        {
            _context.Checkers.Add(_checker);
            _context.CachedCheckers.Add(_cachedChecker);
            await _context.SaveChangesAsync();

            _accessServiceMock.Setup(m => m.CanWrite(It.IsAny<Guid>(),
                It.Is<ApplicationUser>(u => u.Id == _user.Id)))
                .ReturnsAsync(false);

            var result = await _controller.DeleteChecker(_checker.Id);

            Assert.IsAssignableFrom<ForbidResult>(result);
            Assert.Equal(1, _context.Checkers.Count());
            Assert.Equal(1, _context.CachedCheckers.Count());
        }

        [Fact]
        public async Task Found()
        {
            _context.Checkers.Add(_checker);
            _context.CachedCheckers.Add(_cachedChecker);
            await _context.SaveChangesAsync();

            var result = await _controller.DeleteChecker(_checker.Id);

            Assert.Equal(0, _context.Checkers.Count());
            Assert.Equal(0, _context.CachedCheckers.Count());

            var okResult = result as OkObjectResult;
            Assert.NotNull(okResult);

            var testResult = okResult.Value as Checker;
            Assert.Equal(_checker.Id, testResult.Id);
            Assert.Equal(_checker.Name, testResult.Name);
            Assert.Equal(_checker.Code, testResult.Code);
        }
    }
}
