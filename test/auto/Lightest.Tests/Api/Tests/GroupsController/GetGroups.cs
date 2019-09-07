using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Lightest.Data.Models;
using Moq;
using Xunit;

namespace Lightest.Tests.Api.Tests.GroupsController
{
    public class GetGroups : BaseTest
    {
        [Fact]
        public async Task HasAdminAccess()
        {
            AddDataToDb();
            await _context.SaveChangesAsync();

            var result = await _controller.GetGroups();

            var categoriesResult = result as IEnumerable<Group>;

            Assert.NotNull(categoriesResult);
            Assert.Equal(3, categoriesResult.Count());
        }

        [Fact]
        public async Task NoAdminAccess()
        {
            AddDataToDb();
            await _context.SaveChangesAsync();

            _accessServiceMock.Setup(m => m.CheckAdminAccess(It.IsAny<Group>(),
                It.Is<ApplicationUser>(u => u.Id == _user.Id)))
                .Returns(false);

            var result = await _controller.GetGroups();

            var categoriesResult = result as IEnumerable<Group>;

            Assert.NotNull(categoriesResult);
            Assert.Single(categoriesResult);
        }
    }
}
