using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Xunit;

namespace Lightest.Tests.Api.Tests.ContestsController
{
    public class ResetContest : BaseTest
    {
        [Fact]
        public async Task NotFound()
        {
            var result = await _controller.ResetContest(Guid.NewGuid());

            Assert.IsAssignableFrom<NotFoundResult>(result.Result);
        }

        [Fact]
        public async Task DatesReset()
        {
            _contest.StartTime = DateTime.Now;
            _contest.EndTime = DateTime.Now;
            AddDataToDb();
            AddContestToDb();

            var result = await _controller.ResetContest(_contest.CategoryId);
            Assert.Null(result.Value.StartTime);
            Assert.Null(result.Value.EndTime);
            Assert.Equal(_contest.CategoryId, result.Value.CategoryId);

            var contest = _context.Contests.First();

            Assert.Null(contest.StartTime);
            Assert.Null(contest.EndTime);
        }
    }
}
