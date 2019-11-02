using System;
using System.Threading.Tasks;
using Lightest.Data.Models;
using Lightest.Data.Models.TaskModels;
using Lightest.TestingService.Models;
using Moq;
using Xunit;

namespace Lightest.Tests.TestingService.TestingServiceTests
{
    public class ReportResult : BaseTests
    {
        private readonly CheckerResult _result;
        private readonly Upload _upload;
        private readonly TaskDefinition _task;
        private readonly UserTask _userTask;

        public ReportResult()
        {
            _task = new TaskDefinition
            {
                Id = Guid.NewGuid(),
                Points = 100,
                Name = "name"
            };

            _upload = new Upload
            {
                Id = Guid.NewGuid(),
                UserId = Guid.NewGuid().ToString(),
                TaskId = _task.Id
            };

            _result = new CheckerResult
            {
                UploadId = _upload.Id,
                Ip = "1",
                Status = "ResultStatus",
                Message = "ResultMessage",
                SuccessfulTests = 1,
                Type = "Code"
            };

            _userTask = new UserTask
            {
                UserId = _upload.UserId,
                TaskId = _task.Id,
            };

            _context.Tests.AddRange(
                new Test
                {
                    TaskId = _task.Id,
                    Input = "1",
                    Output = "1"
                },
                new Test
                {
                    TaskId = _task.Id,
                    Input = "2",
                    Output = "2"
                }
            );
            _context.Tasks.Add(_task);
            _context.Uploads.Add(_upload);
            _context.UserTasks.Add(_userTask);
            _context.SaveChanges();
        }

        [Fact]
        public async Task UnsupportedType()
        {
            _result.Type = "qwe";
            await _context.SaveChangesAsync();

            await Assert.ThrowsAsync<NotSupportedException>(() => _testingService.ReportResult(_result));
            _serverRepoMock.Verify(s => s.AddFreeServer(It.Is<TestingServer>(ts => ts.Ip == _result.Ip)), Times.Once);
            _serverRepoMock.VerifyNoOtherCalls();
            _factoryMock.VerifyNoOtherCalls();
            _transferMock.VerifyNoOtherCalls();
        }

        [Theory]
        [InlineData(10, 50)]
        [InlineData(60, 60)]
        public async Task NotCompleted(int highScore, int expectedScore)
        {
            _userTask.HighScore = highScore;
            await _context.SaveChangesAsync();

            await _testingService.ReportResult(_result);

            Assert.Equal(50, _upload.Points);
            Assert.Equal(_result.Status, _upload.Status);
            Assert.Equal(_result.Message, _upload.Message);

            Assert.Equal(expectedScore, _userTask.HighScore);
            Assert.False(_userTask.Completed);

            _serverRepoMock.Verify(s => s.AddFreeServer(It.Is<TestingServer>(ts => ts.Ip == _result.Ip)), Times.Once);
            _serverRepoMock.VerifyNoOtherCalls();
            _factoryMock.VerifyNoOtherCalls();
            _transferMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task Completed()
        {
            _result.SuccessfulTests = 2;
            await _context.SaveChangesAsync();

            await _testingService.ReportResult(_result);

            Assert.Equal(100, _upload.Points);
            Assert.Equal(_result.Status, _upload.Status);
            Assert.Equal(_result.Message, _upload.Message);

            Assert.Equal(100, _userTask.HighScore);
            Assert.True(_userTask.Completed);

            _serverRepoMock.Verify(s => s.AddFreeServer(It.Is<TestingServer>(ts => ts.Ip == _result.Ip)), Times.Once);
            _serverRepoMock.VerifyNoOtherCalls();
            _factoryMock.VerifyNoOtherCalls();
            _transferMock.VerifyNoOtherCalls();
        }
    }
}
