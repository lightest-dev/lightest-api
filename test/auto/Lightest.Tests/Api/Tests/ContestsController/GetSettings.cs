using System;
using System.Linq;
using System.Threading.Tasks;
using Lightest.Api.RequestModels.ContestRequests;
using Lightest.Data.Models;
using Microsoft.AspNetCore.Mvc;
using Xunit;

namespace Lightest.Tests.Api.Tests.ContestsController
{
    public class GetSettings : BaseTest
    {
        [Fact]
        public async Task NotFound()
        {
            var result = await _controller.GetSettings(Guid.NewGuid());

            Assert.IsAssignableFrom<NotFoundResult>(result.Result);
        }

        [Fact]
        public async Task CategoryNotContest()
        {
            _category.Contest = false;
            AddDataToDb();
            await _context.SaveChangesAsync();

            var result = await _controller.GetSettings(_category.Id);

            Assert.IsAssignableFrom<BadRequestObjectResult>(result.Result);
        }

        [Fact]
        public async Task DefaultSettingsRetruned()
        {
            AddDataToDb();
            await _context.SaveChangesAsync();

            var result = await _controller.GetSettings(_category.Id);
            Assert.Equal(ContestSettings.Default.Length, result.Value.Length);
        }

        [Fact]
        public async Task StartDateUpdated()
        {
            _contest.StartTime = DateTime.Now;
            _contest.EndTime = DateTime.Now;
            _contest.Length = TimeSpan.FromHours(10);
            AddContestToDb();
            AddDataToDb();
            await _context.SaveChangesAsync();

            var result = await _controller.GetSettings(_category.Id);
            Assert.Equal(_contest.Length, result.Value.Length);
            Assert.Equal(_contest.StartTime, result.Value.StartTime);
            Assert.Equal(_contest.EndTime, result.Value.EndTime);
        }
    }
}
