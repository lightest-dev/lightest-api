using System.Linq;
using System.Threading.Tasks;
using Lightest.Api.RequestModels;
using Lightest.Data.Models;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;

namespace Lightest.Tests.Api.Tests.CheckersController
{
    public class PostChecker : BaseTest
    {
        private readonly CheckerAdd _addModel;

        public PostChecker() => _addModel = new CheckerAdd
        {
            Code = "code",
            Name = "name"
        };

        [Fact]
        public async Task Forbidden()
        {
            _accessServiceMock.Setup(m => m.HasWriteAccess(It.IsAny<Checker>(),
                It.Is<ApplicationUser>(u => u.Id == _user.Id)))
                .Returns(false);

            var result = await _controller.PostChecker(_addModel);

            Assert.IsAssignableFrom<ForbidResult>(result);
            Assert.Equal(0, _context.Tests.Count());
        }

        [Fact]
        public async Task Created()
        {
            var result = await _controller.PostChecker(_addModel);
            var createdAtResult = result as CreatedAtActionResult;
            Assert.NotNull(createdAtResult);

            var checkerResult = createdAtResult.Value as Checker;
            Assert.NotNull(checkerResult);
            Assert.Equal(_addModel.Name, checkerResult.Name);
            Assert.Equal(_addModel.Code, checkerResult.Code);

            Assert.Equal(1, _context.Checkers.Count());
            checkerResult = _context.Checkers.First();
            Assert.Equal(_addModel.Name, checkerResult.Name);
            Assert.Equal(_addModel.Code, checkerResult.Code);
        }
    }
}
