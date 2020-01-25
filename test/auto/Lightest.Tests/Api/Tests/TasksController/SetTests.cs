using System;
using System.Linq;
using System.Threading.Tasks;
using Lightest.Data.Models;
using Lightest.Data.Models.TaskModels;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;

namespace Lightest.Tests.Api.Tests.TasksController
{
    public class SetTests : BaseTest
    {
        private readonly Test _newTest;

        public SetTests()
        {
            _newTest = new Test
            {
                Input = "new_input",
                Output = "new_output",
                Id = Guid.NewGuid()
            };
        }

        [Fact]
        public async Task Forbidden()
        {
            AddDataToDb();
            await _context.SaveChangesAsync();

            _accessServiceMock.Setup(m => m.CanEdit(It.IsAny<Guid>(),
                It.Is<ApplicationUser>(u => u.Id == _user.Id)))
                .ReturnsAsync(false);

            var result = await _controller.SetTests(_task.Id, new[] { _newTest });

            Assert.IsAssignableFrom<ForbidResult>(result);

            var task = _context.Tasks.First();
            Assert.Single(task.Tests);
            var test = task.Tests.First();

            Assert.Equal(_task.Id, test.TaskId);
            Assert.Equal(_test.Input, test.Input);
            Assert.Equal(_test.Output, test.Output);
        }

        [Fact]
        public async Task NotFound()
        {
            AddDataToDb();
            await _context.SaveChangesAsync();

            var result = await _controller.SetTests(Guid.NewGuid(), new[] { _newTest });

            Assert.IsAssignableFrom<NotFoundResult>(result);

            var task = _context.Tasks.First();
            Assert.Single(task.Tests);
            var test = task.Tests.First();

            Assert.Equal(_task.Id, test.TaskId);
            Assert.Equal(_test.Input, test.Input);
            Assert.Equal(_test.Output, test.Output);
        }

        [Fact]
        public async Task LanguageSet()
        {
            AddDataToDb();
            await _context.SaveChangesAsync();

            var result = await _controller.SetTests(_task.Id, new[] { _newTest });

            Assert.IsAssignableFrom<OkResult>(result);

            var task = _context.Tasks.First();
            Assert.Single(task.Tests);
            var test = task.Tests.First();

            Assert.Equal(_task.Id, test.TaskId);
            Assert.Equal(_newTest.Input, test.Input);
            Assert.Equal(_newTest.Output, test.Output);
            Assert.Equal(_newTest.Id, test.Id);
        }
    }
}
