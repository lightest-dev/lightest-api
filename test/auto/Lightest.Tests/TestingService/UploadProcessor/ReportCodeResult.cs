using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GrpcServices;
using Lightest.Data.Models;
using Xunit;

namespace Lightest.Tests.TestingService.UploadProcessor
{
    public class ReportCodeResult : BaseTest
    {
        private readonly TestingResponse _result;
        private readonly Assignment _userTask;

        public ReportCodeResult()
        {
            _userTask = new Assignment
            {
                UserId = _upload.UserId,
                TaskId = _task.Id,
            };

            _task.Tests = new List<Data.Models.TaskModels.Test>
            {
                new Data.Models.TaskModels.Test
                {
                    Id = Guid.NewGuid()
                },
                new Data.Models.TaskModels.Test
                {
                    Id = Guid.NewGuid()
                }
            };

            _result = new TestingResponse
            {
                UploadId = _upload.Id.ToString(),
                Status = "ResultStatus",
                Message = "ResultMessage",
                SuccessfulTestsCount = 1,
                FailedTestsCount = 1,
                TestingFailed = false
            };
        }

        protected override Task SetUpData()
        {
            _context.UserTasks.Add(_userTask);

            return base.SetUpData();
        }

        [Theory]
        [InlineData(10, 50)]
        [InlineData(60, 60)]
        public async Task NotCompleted(int highScore, int expectedScore)
        {
            _userTask.HighScore = highScore;
            await SetUpData();

            await _uploadProcessor.ReportCodeResult(_result);

            var upload = _context.Uploads.First(u => u.Id == _upload.Id);

            Assert.Equal(50, upload.Points);
            Assert.Equal(_result.Status, upload.Status);
            Assert.Equal(_result.Message, upload.Message);

            var asignment = _context.UserTasks.First(a => a.UserId == _upload.UserId &&
                a.TaskId == _task.Id);
            Assert.Equal(expectedScore, asignment.HighScore);
            Assert.False(asignment.Completed);
        }

        [Fact]
        public async Task Completed()
        {
            _result.SuccessfulTestsCount = 2;
            _result.FailedTestsCount = 0;
            await SetUpData();

            await _uploadProcessor.ReportCodeResult(_result);

            var upload = _context.Uploads.First(u => u.Id == _upload.Id);

            Assert.Equal(100, upload.Points);
            Assert.Equal(_result.Status, upload.Status);
            Assert.Equal(_result.Message, upload.Message);

            var asignment = _context.UserTasks.First(a => a.UserId == _upload.UserId &&
                a.TaskId == _task.Id);
            Assert.Equal(100, asignment.HighScore);
            Assert.True(asignment.Completed);
        }
    }
}
