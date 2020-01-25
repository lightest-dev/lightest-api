using System;
using System.Linq;
using System.Threading.Tasks;
using Lightest.Data.Models;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;

namespace Lightest.Tests.Api.Tests.LanguagesController
{
    public class PostLanguage : BaseTest
    {
        [Fact]
        public async Task Forbidden()
        {
            _accessServiceMock.Setup(m => m.CanEdit(It.IsAny<Guid>(),
                It.Is<ApplicationUser>(u => u.Id == _user.Id)))
                .ReturnsAsync(false);

            var result = await _controller.PostLanguage(_language);
            Assert.IsAssignableFrom<ForbidResult>(result);

            Assert.Empty(_context.Languages);
        }

        [Fact]
        public async Task Added()
        {
            var result = await _controller.PostLanguage(_language);

            var okResult = result as OkObjectResult;
            Assert.NotNull(okResult);

            var id = (Guid)okResult.Value;
            Assert.Equal(_language.Id, id);

            Assert.Single(_context.Languages);
            var language = _context.Languages.First();
            Assert.Equal(_language.Id, language.Id);
            Assert.Equal(_language.Name, language.Name);
            Assert.Equal(_language.Extension, language.Extension);
        }
    }
}
