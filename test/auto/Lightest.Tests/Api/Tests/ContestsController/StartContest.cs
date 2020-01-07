using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Xunit;

namespace Lightest.Tests.Api.Tests.ContestsController
{
    public class StartContest : BaseTest
    {
        [Fact]
        public async Task NotFound()
        {
            var result = await _controller.StartContest(Guid.NewGuid());

            Assert.IsAssignableFrom<NotFoundResult>(result.Result);
        }

        [Fact]
        public async Task CategoryNotContest()
        {
            _category.Contest = false;
            AddDataToDb();
            await _context.SaveChangesAsync();

            var result = await _controller.StartContest(_category.Id);

            Assert.IsAssignableFrom<BadRequestObjectResult>(result.Result);
        }

        [Fact]
        public async Task DefaultSettingsApplied()
        {
            AddDataToDb();
            await _context.SaveChangesAsync();

            var result = await _controller.StartContest(_category.Id);
            // TODO
        }
    }
}
