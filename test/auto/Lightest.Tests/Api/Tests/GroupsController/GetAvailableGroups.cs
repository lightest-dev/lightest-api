using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Lightest.Api.ResponseModels.GroupViews;
using Lightest.Data.Models;
using Xunit;

namespace Lightest.Tests.Api.Tests.GroupsController
{
    public class GetAvailableGroups : BaseTest
    {
        [Fact]
        public async Task HasAssignedGroup()
        {
            var parentUser = new UserGroup
            {
                UserId = _user.Id,
                GroupId = _parent.Id,
                User = _user,
                Group = _parent,
                CanRead = false,
                CanWrite = true,
                CanChangeAccess = true
            };
            _parent.Users = new List<UserGroup>
            {
                parentUser
            };

            AddDataToDb();
            await _context.SaveChangesAsync();

            var result = await _controller.GetAvailableGroups(new Sieve.Models.SieveModel());

            var groupsResult = result as IEnumerable<ListGroupView>;

            Assert.NotNull(groupsResult);

            var group = groupsResult.Single();
            Assert.Equal(_parent.Name, group.Name);
            Assert.Equal(_parent.Id, group.Id);
            Assert.Equal(parentUser.CanRead, group.CanRead);
            Assert.Equal(parentUser.CanWrite, group.CanWrite);
            Assert.Equal(parentUser.CanChangeAccess, group.CanChangeAccess);
        }

        [Fact]
        public async Task NoAssignedGroup()
        {
            AddDataToDb();
            await _context.SaveChangesAsync();

            var result = await _controller.GetAvailableGroups(new Sieve.Models.SieveModel());

            var groupsResult = result as IEnumerable<ListGroupView>;

            Assert.NotNull(groupsResult);
            Assert.Empty(groupsResult);
        }
    }
}
