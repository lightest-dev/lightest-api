using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Lightest.Data.Models;
using Microsoft.AspNetCore.Mvc;
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

            var result = await _controller.GetGroups(new Sieve.Models.SieveModel());

            var okResult = result as OkObjectResult;
            Assert.NotNull(okResult);

            var groupsResult = okResult.Value as IEnumerable<Group>;

            Assert.NotNull(groupsResult);
            Assert.Equal(3, groupsResult.Count());
        }

        [Fact]
        public async Task NoAdminAccess()
        {
            AddDataToDb();
            await _context.SaveChangesAsync();

            _roleHelper.Setup(m => m.IsAdmin(It.Is<ApplicationUser>(u => u.Id == _user.Id)))
                .ReturnsAsync(false);

            var result = await _controller.GetGroups(new Sieve.Models.SieveModel());

            Assert.IsAssignableFrom<ForbidResult>(result);
        }
    }
}
