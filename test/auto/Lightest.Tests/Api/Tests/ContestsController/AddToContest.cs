using System;
using System.Threading.Tasks;
using Lightest.Api.RequestModels.ContestRequests;
using Lightest.Data.Models;
using Microsoft.AspNetCore.Mvc;
using Xunit;

namespace Lightest.Tests.Api.Tests.ContestsController
{
    public class AddToContest : BaseTest
    {
        private ApplicationUser TUser1 { get; }
        private ApplicationUser TUser2 { get; }
        private ApplicationUser XUser1 { get; }

        private AddToContestByNameRequest Request { get; }

        private void AddUsers()
        {
            _context.Users.Add(TUser1);
            _context.Users.Add(TUser2);
            _context.Users.Add(XUser1);
        }

        public AddToContest()
        {
            TUser1 = new ApplicationUser
            {
                Id = Guid.NewGuid().ToString(),
                Name = nameof(TUser1)
            };
            TUser2 = new ApplicationUser
            {
                Id = Guid.NewGuid().ToString(),
                Name = nameof(TUser2)
            };
            XUser1 = new ApplicationUser
            {
                Id = Guid.NewGuid().ToString(),
                Name = nameof(XUser1)
            };

            Request = new AddToContestByNameRequest
            {
                ContestId = _category.Id,
                Pattern = nameof(AddToContestByNameRequest.Pattern)
            };
        }

        [Theory]
        [InlineData(default)]
        [InlineData("")]
        public async Task PatternNotSet(string pattern)
        {
            Request.Pattern = pattern;

            var result = await _controller.AddToContest(Request);

            Assert.IsAssignableFrom<BadRequestObjectResult>(result.Result);
        }

        [Fact]
        public async Task CategoryNotSet()
        {
            Request.ContestId = default;

            var result = await _controller.AddToContest(Request);

            Assert.IsAssignableFrom<BadRequestObjectResult>(result.Result);
        }

        [Fact]
        public async Task CategoryNotFound()
        {
            Request.ContestId = Guid.NewGuid();

            var result = await _controller.AddToContest(Request);

            Assert.IsAssignableFrom<NotFoundObjectResult>(result.Result);
        }

        [Fact]
        public async Task UsersAdded()
        {
            Request.Pattern = "TUs%";
            _category.Users.Add(new CategoryUser
            {
                UserId = TUser1.Id
            });
            AddDataToDb();
            AddUsers();
            await _context.SaveChangesAsync();

            var result = await _controller.AddToContest(Request);

            // Does not work with in-memory db. Should be covered by integration tests.
            //var addedUser = result.Value.Single();
            //Assert.Equal(TUser2.Id, addedUser.UserId);

            //var users = _context.Categories.Single().Users;
            //Assert.Contains(users, u => u.UserId == TUser2.Id && u.CanRead == true);
            Assert.NotNull(result.Value);
        }
    }
}
