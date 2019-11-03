using System;
using System.Linq;
using System.Threading.Tasks;
using Lightest.Api.RequestModels.CheckerRequests;
using Lightest.Data.Models;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;

namespace Lightest.Tests.Api.Tests.CheckersController
{
    public class PutChecker : BaseTest
    {
        private readonly UpdateCheckerRequest _modifiedChecker;

        public PutChecker() => _modifiedChecker = new UpdateCheckerRequest
        {
            Id = _checker.Id,
            Code = "code2",
            Name = "name2"
        };

        [Fact]
        public async Task IdsDontMatch()
        {
            var result = await _controller.PutChecker(Guid.NewGuid(), _modifiedChecker);

            Assert.IsAssignableFrom<BadRequestResult>(result);
        }

        [Fact]
        public async Task NoTestFound()
        {
            var result = await _controller.PutChecker(_modifiedChecker.Id, _modifiedChecker);

            Assert.IsAssignableFrom<NotFoundResult>(result);
        }

        [Fact]
        public async Task Forbidden()
        {
            _context.Checkers.Add(_checker);
            _context.CachedCheckers.Add(_cachedChecker);
            await _context.SaveChangesAsync();

            _accessServiceMock.Setup(m => m.HasWriteAccess(It.IsAny<Checker>(),
                It.Is<ApplicationUser>(u => u.Id == _user.Id)))
                .Returns(false);

            var result = await _controller.PutChecker(_modifiedChecker.Id, _modifiedChecker);

            Assert.IsAssignableFrom<ForbidResult>(result);
            Assert.Equal(1, _context.Checkers.Count());
            Assert.Equal(1, _context.CachedCheckers.Count());

            var checkerResult = _context.Checkers.First();
            Assert.Equal(_checker.Id, checkerResult.Id);
            Assert.Equal(_checker.Name, checkerResult.Name);
            Assert.Equal(_checker.Code, checkerResult.Code);
        }

        [Fact]
        public async Task Modified()
        {
            _checker.Compiled = true;
            _checker.Message = "message";
            _context.Checkers.Add(_checker);
            _context.CachedCheckers.Add(_cachedChecker);
            await _context.SaveChangesAsync();

            var result = await _controller.PutChecker(_modifiedChecker.Id, _modifiedChecker);
            Assert.IsAssignableFrom<OkResult>(result);

            Assert.Equal(1, _context.Checkers.Count());
            Assert.Equal(0, _context.CachedCheckers.Count());

            var testResult = _context.Checkers.First();
            Assert.Equal(_checker.Id, testResult.Id);
            Assert.Equal(_modifiedChecker.Name, testResult.Name);
            Assert.Equal(_modifiedChecker.Code, testResult.Code);
            Assert.False(testResult.Compiled);
            Assert.Null(testResult.Message);
        }
    }
}
