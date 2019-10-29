using System;
using System.Linq;
using System.Threading.Tasks;
using Lightest.Data.Models;
using Lightest.Data.Models.TaskModels;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;

namespace Lightest.Tests.Api.Tests.LanguagesController
{
    public class DeleteLanguage : BaseTest
    {
        [Fact]
        public async Task NotFound()
        {
            throw new NotImplementedException();
            _context.Languages.Add(_language);
            await _context.SaveChangesAsync();

            var result = await _controller.DeleteLanguage(Guid.NewGuid());
            Assert.IsAssignableFrom<NotFoundResult>(result);

            Assert.Single(_context.Languages);
            var language = _context.Languages.First();
            Assert.Equal(_language.Id, language.Id);
            Assert.Equal(_language.Name, language.Name);
            Assert.Equal(_language.Extension, language.Extension);
        }

        [Fact]
        public async Task Forbidden()
        {
            _context.Languages.Add(_language);
            await _context.SaveChangesAsync();

            _accessServiceMock.Setup(m => m.CheckWriteAccess(It.IsAny<Language>(),
                It.Is<ApplicationUser>(u => u.Id == _user.Id)))
                .Returns(false);

            var result = await _controller.DeleteLanguage(_language.Id);
            Assert.IsAssignableFrom<ForbidResult>(result);

            Assert.Single(_context.Languages);
            var language = _context.Languages.First();
            Assert.Equal(_language.Id, language.Id);
            Assert.Equal(_language.Name, language.Name);
            Assert.Equal(_language.Extension, language.Extension);
        }

        [Fact]
        public async Task Added()
        {
            _context.Languages.Add(_language);
            await _context.SaveChangesAsync();

            var result = await _controller.DeleteLanguage(_language.Id);

            var okResult = result as OkObjectResult;
            Assert.NotNull(okResult);

            var language = okResult.Value as Language;
            Assert.Equal(_language.Id, language.Id);
            Assert.Equal(_language.Name, language.Name);
            Assert.Equal(_language.Extension, language.Extension);

            Assert.Empty(_context.Languages);
        }
    }
}
