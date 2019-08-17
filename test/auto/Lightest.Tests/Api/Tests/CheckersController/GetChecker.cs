using System;
using System.Threading.Tasks;
using Lightest.Data.Models;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;

namespace Lightest.Tests.Api.Tests.CheckersController
{
    public class GetChecker : BaseTest
    {
        [Fact]
        public async Task NoCheckerFound()
        {
            var result = await _controller.GetChecker(Guid.NewGuid());

            Assert.IsAssignableFrom<NotFoundResult>(result);
        }

        [Fact]
        public async Task Forbidden()
        {
            _context.Checkers.Add(_checker);
            await _context.SaveChangesAsync();

            _accessServiceMock.Setup(m => m.CheckReadAccess(It.IsAny<Checker>(),
                It.Is<ApplicationUser>(u => u.Id == _user.Id)))
                .Returns(false);

            var result = await _controller.GetChecker(_checker.Id);

            Assert.IsAssignableFrom<ForbidResult>(result);
        }

        [Fact]
        public async Task Found()
        {
            _context.Checkers.Add(_checker);
            await _context.SaveChangesAsync();

            var result = await _controller.GetChecker(_checker.Id);

            var okResult = result as OkObjectResult;
            Assert.NotNull(okResult);

            var checkerResult = okResult.Value as Checker;
            Assert.Equal(_checker.Id, checkerResult.Id);
            Assert.Equal(_checker.Name, checkerResult.Name);
            Assert.Equal(_checker.Code, checkerResult.Code);
        }
    }
}
