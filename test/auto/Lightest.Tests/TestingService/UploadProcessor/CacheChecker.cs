using System;
using System.Linq;
using System.Threading.Tasks;
using GrpcServices;
using Lightest.Data.Models;
using Moq;
using Xunit;

namespace Lightest.Tests.TestingService.UploadProcessor
{
    public class CacheChecker : BaseTest
    {
        private readonly Checker _checker;

        public CacheChecker()
        {
            _checker = new Checker
            {
                Id = Guid.NewGuid(),
                Code = nameof(_checker)
            };

            _task.Checker = _checker;
        }

        protected override Task SetUpData()
        {
            _context.Checkers.Add(_checker);

            return base.SetUpData();
        }

        [Fact]
        public async Task CheckerCached()
        {
            _context.CachedCheckers.Add(new ServerChecker
            {
                CheckerId = _checker.Id,
                ServerIp = _serverInfo.Ip
            });

            await SetUpData();

            var result = await _uploadProcessor.CacheChecker();

            _transferService.Verify(ts => ts.SendChecker(It.IsAny<CheckerRequest>()),
                Times.Never);

            Assert.True(result, $"{nameof(CacheChecker)} should return true if checker is cached.");
        }

        [Fact]
        public async Task CompilationFailed()
        {
            const string errorMessage = "Compilation Error";
            await SetUpData();
            _transferService.Setup(ts => ts.SendChecker(It.Is<CheckerRequest>(
                r => r.Id == _checker.Id.ToString() && r.Code == _checker.Code)))
                .ReturnsAsync(new CheckerResponse
                {
                    Compiled = false,
                    Message = errorMessage
                });

            var result = await _uploadProcessor.CacheChecker();

            Assert.False(result, $"{nameof(CacheChecker)} should return false if checker compilation failed.");
            Assert.Empty(_context.CachedCheckers);

            var checker = _context.Checkers.First(c => c.Id == _checker.Id);
            Assert.False(checker.Compiled);
            Assert.Equal(errorMessage, checker.Message);
        }

        [Fact]
        public async Task CompilationSuccessful()
        {
            const string compilationMessage = "Compilation Successful";
            await SetUpData();
            _transferService.Setup(ts => ts.SendChecker(It.Is<CheckerRequest>(
                r => r.Id == _checker.Id.ToString() && r.Code == _checker.Code)))
                .ReturnsAsync(new CheckerResponse
                {
                    Compiled = true,
                    Message = compilationMessage
                });

            var result = await _uploadProcessor.CacheChecker();

            Assert.True(result, $"{nameof(CacheChecker)} should return true if checker compilation succeded.");
            var cachedChecker = _context.CachedCheckers.First();
            Assert.Equal(_serverInfo.Ip, cachedChecker.ServerIp);
            Assert.Equal(_checker.Id, cachedChecker.CheckerId);

            var checker = _context.Checkers.First(c => c.Id == _checker.Id);
            Assert.True(checker.Compiled);
            Assert.Equal(compilationMessage, checker.Message);
        }
    }
}
