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
    public class Put : BaseTest
    {
        private readonly Test _modifiedTest;

        public Put() => _modifiedTest = new Test
        {
            Id = _test.Id,
            Input = "new_input",
            Output = "new_output"
        };

        [Fact]
        public async Task IdsDontMatch()
        {
            var result = await _controller.PutTest(Guid.NewGuid(), _modifiedTest);

            Assert.IsAssignableFrom<BadRequestResult>(result);
        }

        [Fact]
        public async Task NoTestFound()
        {
            var result = await _controller.PutTest(_modifiedTest.Id, _modifiedTest);

            Assert.IsAssignableFrom<NotFoundResult>(result);
        }

        [Fact]
        public async Task Forbidden()
        {
            _context.Tests.Add(_test);
            await _context.SaveChangesAsync();

            _accessServiceMock.Setup(m => m.HasWriteAccess(It.IsAny<TaskDefinition>(),
                It.Is<ApplicationUser>(u => u.Id == _user.Id)))
                .Returns(false);

            var result = await _controller.PutTest(_modifiedTest.Id, _modifiedTest);

            Assert.IsAssignableFrom<ForbidResult>(result);
            Assert.Equal(1, _context.Tests.Count());

            var testResult = _context.Tests.First();
            Assert.Equal(_test.Id, testResult.Id);
            Assert.Equal(_test.TaskId, testResult.TaskId);
            Assert.Equal(_test.Input, testResult.Input);
            Assert.Equal(_test.Output, testResult.Output);
        }

        [Fact]
        public async Task Modified()
        {
            _context.Tests.Add(_test);
            await _context.SaveChangesAsync();

            var result = await _controller.PutTest(_modifiedTest.Id, _modifiedTest);
            Assert.IsAssignableFrom<OkResult>(result);

            Assert.Equal(1, _context.Tests.Count());

            var testResult = _context.Tests.First();
            Assert.Equal(_test.Id, testResult.Id);
            Assert.Equal(_test.TaskId, testResult.TaskId);
            Assert.Equal(_modifiedTest.Input, testResult.Input);
            Assert.Equal(_modifiedTest.Output, testResult.Output);
        }
    }
}
