using System;
using System.Linq;
using System.Threading.Tasks;
using Lightest.Data.Models;
using Lightest.Data.Models.TaskModels;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;

namespace Lightest.Tests.Api.Tests.TestsController
{
    public class Delete : BaseTest
    {
        [Fact]
        public async Task NoTestFound()
        {
            var result = await _controller.DeleteTest(Guid.NewGuid());

            Assert.IsAssignableFrom<NotFoundResult>(result);
        }

        [Fact]
        public async Task Forbidden()
        {
            _context.Tests.Add(_test);
            await _context.SaveChangesAsync();

            _accessServiceMock.Setup(m => m.CheckWriteAccess(It.IsAny<TaskDefinition>(),
                It.Is<ApplicationUser>(u => u.Id == _user.Id)))
                .Returns(false);

            var result = await _controller.DeleteTest(_test.Id);

            Assert.IsAssignableFrom<ForbidResult>(result);
            Assert.Equal(1, _context.Tests.Count());
        }

        [Fact]
        public async Task Found()
        {
            _context.Tests.Add(_test);
            await _context.SaveChangesAsync();

            var result = await _controller.DeleteTest(_test.Id);

            Assert.Equal(0, _context.Tests.Count());

            var okResult = result as OkObjectResult;
            Assert.NotNull(okResult);

            var testResult = okResult.Value as Test;
            Assert.Equal(_test.Id, testResult.Id);
            Assert.Equal(_test.TaskId, testResult.TaskId);
            Assert.Equal(_test.Input, testResult.Input);
            Assert.Equal(_test.Output, testResult.Output);
        }
    }
}
