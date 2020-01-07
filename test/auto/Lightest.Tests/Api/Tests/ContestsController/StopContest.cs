using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Xunit;

namespace Lightest.Tests.Api.Tests.ContestsController
{
    public class StopContest : BaseTest
    {
        [Fact]
        public async Task NotFound()
        {
            var result = await _controller.StopContest(Guid.NewGuid());

            Assert.IsAssignableFrom<NotFoundResult>(result.Result);
        }

        [Fact]
        public async Task DatesReset()
        {
            _contest.StartTime = DateTime.Now;
            _contest.EndTime = null;
            AddDataToDb();
            AddContestToDb();

            var result = await _controller.StopContest(_contest.CategoryId);
            Assert.NotNull(result.Value.EndTime);
            Assert.Equal(_contest.CategoryId, result.Value.CategoryId);

            var contest = _context.Contests.First();

            Assert.NotNull(contest.EndTime);
        }
    }
}
