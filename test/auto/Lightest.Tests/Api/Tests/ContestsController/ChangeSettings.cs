using System;
using System.Linq;
using System.Threading.Tasks;
using Lightest.Api.RequestModels.ContestRequests;
using Lightest.Data.Models;
using Microsoft.AspNetCore.Mvc;
using Xunit;

namespace Lightest.Tests.Api.Tests.ContestsController
{
    public class ChangeSettings : BaseTest
    {
        private UpdateSettingsRequest Request { get; }

        public ChangeSettings()
        {
            Request = new UpdateSettingsRequest
            {
                Length = TimeSpan.FromHours(10),
                StartTime = DateTime.Now
            };
        }

        [Fact]
        public async Task NotFound()
        {
            var result = await _controller.ChangeSettings(Guid.NewGuid(), Request);

            Assert.IsAssignableFrom<NotFoundResult>(result.Result);
        }

        [Fact]
        public async Task CategoryNotContest()
        {
            _category.Contest = false;
            AddDataToDb();
            await _context.SaveChangesAsync();

            var result = await _controller.ChangeSettings(_category.Id, Request);

            Assert.IsAssignableFrom<BadRequestObjectResult>(result.Result);
        }

        [Fact]
        public async Task DefaultSettingsApplied()
        {
            Request.Length = null;
            AddDataToDb();
            await _context.SaveChangesAsync();

            var result = await _controller.ChangeSettings(_category.Id, Request);
            Assert.Null(result.Value.Length);
            Assert.Equal(Request.StartTime, result.Value.StartTime);

            var settings = _context.Contests.First();
            Assert.Null(settings.Length);
            Assert.Equal(Request.StartTime, result.Value.StartTime);
        }

        [Fact]
        public async Task StartDateUpdated()
        {
            AddContestToDb();
            AddDataToDb();
            await _context.SaveChangesAsync();

            var result = await _controller.ChangeSettings(_category.Id, Request);
            Assert.Equal(Request.Length, result.Value.Length);
            Assert.Equal(Request.StartTime, result.Value.StartTime);
            Assert.Equal(result.Value.EndTime, result.Value.StartTime + result.Value.Length);

            var settings = _context.Contests.First();
            Assert.Equal(Request.Length, settings.Length);
            Assert.Equal(Request.StartTime, result.Value.StartTime);
            Assert.Equal(settings.EndTime, settings.StartTime + settings.Length);
        }
    }
}
