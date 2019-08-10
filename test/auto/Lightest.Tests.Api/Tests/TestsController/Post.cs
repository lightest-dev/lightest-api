using System.Linq;
using System.Threading.Tasks;
using Lightest.Data.Models;
using Lightest.Data.Models.TaskModels;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;

namespace Lightest.Tests.Api.Tests.TestsController
{
    public class Post : BaseTest
    {
        [Fact]
        public async Task NoTaskFound()
        {
            var result = await _controller.PostTest(_test);

            Assert.IsAssignableFrom<BadRequestResult>(result);
        }

        [Fact]
        public async Task Forbidden()
        {
            _context.Tasks.Add(_test.Task);
            await _context.SaveChangesAsync();

            _accessServiceMock.Setup(m => m.CheckWriteAccess(It.IsAny<TaskDefinition>(),
                It.Is<ApplicationUser>(u => u.Id == _user.Id)))
                .Returns(false);

            var result = await _controller.PostTest(_test);

            Assert.IsAssignableFrom<ForbidResult>(result);
            Assert.Equal(0, _context.Tests.Count());
        }

        [Fact]
        public async Task Created()
        {
            _context.Tasks.Add(_test.Task);
            _test.Task = null;
            await _context.SaveChangesAsync();

            var result = await _controller.PostTest(_test);
            var createdAtResult = result as CreatedAtActionResult;
            Assert.NotNull(createdAtResult);

            var testResult = createdAtResult.Value as Test;
            Assert.NotNull(testResult);
            Assert.Equal(_test.Id, testResult.Id);
            Assert.Equal(_test.TaskId, testResult.TaskId);
            Assert.Equal(_test.Input, testResult.Input);
            Assert.Equal(_test.Output, testResult.Output);

            var routeId = createdAtResult.RouteValues["id"];
            Assert.Equal(_test.Id, routeId);

            Assert.Equal(1, _context.Tests.Count());
            testResult = _context.Tests.First();
            Assert.Equal(_test.Id, testResult.Id);
            Assert.Equal(_test.TaskId, testResult.TaskId);
            Assert.Equal(_test.Input, testResult.Input);
            Assert.Equal(_test.Output, testResult.Output);
        }
    }
}
