using System.Linq;
using Xunit;

namespace Lightest.Tests.Api.Tests.LanguagesController
{
    public class GetLanguages : BaseTest
    {
        [Fact]
        public void DbEmpty()
        {
            var result = _controller.GetLanguages();
            Assert.Empty(result);
        }

        [Fact]
        public void LanguagePresent()
        {
            _context.Languages.Add(_language);
            _context.SaveChanges();

            var result = _controller.GetLanguages();
            Assert.Single(result);

            var language = result.First();
            Assert.Equal(_language.Id, language.Id);
            Assert.Equal(_language.Name, language.Name);
            Assert.Equal(_language.Extension, language.Extension);
        }
    }
}
