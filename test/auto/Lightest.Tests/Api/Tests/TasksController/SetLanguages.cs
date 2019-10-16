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
    public class SetLanguages : BaseTest
    {
        protected readonly TaskLanguage _taskLanguage;

        public SetLanguages() => _taskLanguage = new TaskLanguage
        {
            LanguageId = Guid.NewGuid(),
            MemoryLimit = 500,
            TimeLimit = 500
        };

        [Fact]
        public async Task Forbidden()
        {
            AddDataToDb();
            await _context.SaveChangesAsync();

            _accessServiceMock.Setup(m => m.CheckWriteAccess(It.IsAny<TaskDefinition>(),
                It.Is<ApplicationUser>(u => u.Id == _user.Id)))
                .Returns(false);

            var result = await _controller.SetLanguages(_task.Id, new[] { _taskLanguage });

            Assert.IsAssignableFrom<ForbidResult>(result);

            var task = _context.Tasks.First();
            Assert.Single(task.Languages);
            var language = task.Languages.First();
            Assert.Equal(_language.Id, language.LanguageId);
        }

        [Fact]
        public async Task NotFound()
        {
            AddDataToDb();
            await _context.SaveChangesAsync();

            var result = await _controller.SetLanguages(Guid.NewGuid(), new[] { _taskLanguage });

            Assert.IsAssignableFrom<NotFoundResult>(result);

            var task = _context.Tasks.First();
            Assert.Single(task.Languages);
            var language = task.Languages.First();
            Assert.Equal(_language.Id, language.LanguageId);
        }

        [Fact]
        public async Task LanguageSet()
        {
            AddDataToDb();
            await _context.SaveChangesAsync();

            var result = await _controller.SetLanguages(_task.Id, new[] { _taskLanguage });

            Assert.IsAssignableFrom<OkResult>(result);

            var task = _context.Tasks.First();
            Assert.Single(task.Languages);
            var language = task.Languages.First();

            Assert.Equal(_taskLanguage.LanguageId, language.LanguageId);
            Assert.Equal(_taskLanguage.MemoryLimit, language.MemoryLimit);
            Assert.Equal(_taskLanguage.TimeLimit, language.TimeLimit);
            Assert.Equal(_task.Id, language.TaskId);
        }
    }
}
