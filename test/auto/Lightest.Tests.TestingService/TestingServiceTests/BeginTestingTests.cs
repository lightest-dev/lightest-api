using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Lightest.Data.Models;
using Lightest.Data.Models.TaskModels;
using Lightest.TestingService.Requests;
using Moq;
using Xunit;

namespace Lightest.Tests.TestingService.TestingServiceTests
{
    //todo: Check test input sending failure
    //todo: Check test output sending failure
    public class BeginTestingTests : BaseTests
    {
        private readonly TestingServer _testServer;

        private readonly Checker _checker;

        private readonly Language _language;

        private readonly TaskDefinition _task;

        private readonly CodeUpload _upload;

        public BeginTestingTests()
        {
            _testServer = new TestingServer
            {
                Ip = "1",
                Status = ServerStatus.Free,
                Port = 2,
                Version = "1"
            };

            _language = new Language
            {
                Id = Guid.NewGuid(),
                Name = "name",
                Extension = ".ext"
            };

            _checker = new Checker
            {
                Id = Guid.NewGuid(),
                Name = "test",
                Code = "code"
            };

            _task = new TaskDefinition
            {
                Id = Guid.NewGuid(),
                Public = true,
                Points = 100,
                Checker = _checker,
                CheckerId = _checker.Id,
                Name = "name",
                Tests = new List<Test>(),
                Languages = new List<TaskLanguage>()
            };

            _task.Languages.Add(new TaskLanguage
            {
                Language = _language,
                LanguageId = _language.Id,
                Task = _task,
                TaskId = _task.Id,
                MemoryLimit = 512,
                TimeLimit = 1000
            });

            _upload = new CodeUpload
            {
                UploadId = Guid.NewGuid(),
                Code = "code",
                Task = _task,
                TaskId = _task.Id,
                Language = _language,
                LanguageId = _language.Id
            };

            _transferMock.Setup(t => t.SendMessage(It.IsNotNull<string>()))
                .Returns(Task.FromResult(true));

            _transferMock.Setup(t => t.SendFile(It.IsNotNull<FileRequest>(), It.IsNotNull<byte[]>()))
                .Returns(Task.FromResult(true));

            _serverRepoMock.Setup(r => r.GetFreeServer())
                .Returns(_testServer);

        }

        [Fact]
        public async Task NoFreeServerTest()
        {
            _serverRepoMock.Setup(r => r.GetFreeServer())
                .Returns(default(TestingServer));

            var result = await _testingService.BeginTesting(_upload);

            Assert.False(result);

            _serverRepoMock.Verify(s => s.GetFreeServer(), Times.Once);
        }

        [Fact]
        public async Task CachingFailedTest()
        {
            _transferMock.Setup(t => t.SendMessage(It.IsNotNull<string>()))
                .Returns(Task.FromResult(false));

            var result = await _testingService.BeginTesting(_upload);

            Assert.False(result);
            _serverRepoMock.Verify(s => s.GetFreeServer(), Times.Once);
            _transferMock.Verify(t => t.SendMessage(It.IsNotNull<string>()), Times.Once);

            Assert.Equal(1, _context.CodeUploads.Count());
            var upload = _context.CodeUploads.First();
            Assert.Equal(UploadStatus.Queue, upload.Status);
        }

        [Fact]
        public async Task UploadSendingFailedTest()
        {
            _transferMock.Setup(t => t.SendFile(It.IsNotNull<FileRequest>(), It.IsNotNull<byte[]>()))
                .Returns(Task.FromResult(false));

            var result = await _testingService.BeginTesting(_upload);

            Assert.False(result);
            _serverRepoMock.Verify(s => s.GetFreeServer(), Times.Once);
            _transferMock.Verify(t => t.SendMessage(It.IsNotNull<string>()), Times.Exactly(2));
            _transferMock.Verify(t => t.SendFile(
                It.IsNotNull<FileRequest>(), It.IsNotNull<byte[]>()), Times.Once);
            _serverRepoMock.Verify(sr => sr.AddBrokenServer(It.Is<TestingServer>(s => s.Ip == _testServer.Ip)), Times.Once);

            Assert.Equal(1, _context.CodeUploads.Count());
            var upload = _context.CodeUploads.First();
            Assert.Equal(UploadStatus.Queue, upload.Status);
        }

        [Fact]
        public async Task CachedCheckerTest()
        {
            _context.CachedCheckers.Add(new ServerChecker
            {
                Checker = _checker,
                CheckerId = _checker.Id,
                Server = _testServer,
                ServerIp = _testServer.Ip
            });
            _context.SaveChanges();

            var result = await _testingService.BeginTesting(_upload);
            Assert.True(result);

            _serverRepoMock.Verify(s => s.GetFreeServer(), Times.Once);
            _transferMock.Verify(t => t.SendMessage(It.IsNotNull<string>()), Times.Once);

            Assert.Equal(1, _context.CodeUploads.Count());
            var upload = _context.CodeUploads.First();
            Assert.Equal(UploadStatus.Testing, upload.Status);

            _transferMock.Verify(t => t.SendFile(It.IsNotNull<FileRequest>(), It.IsNotNull<byte[]>()), Times.Once);
        }

        [Fact]
        public async Task NonCachedCheckerTest()
        {
            var result = await _testingService.BeginTesting(_upload);
            Assert.True(result);

            _serverRepoMock.Verify(s => s.GetFreeServer(), Times.Once);
            _transferMock.Verify(t => t.SendMessage(It.IsNotNull<string>()), Times.Exactly(2));
            _serverRepoMock.Verify(sr => sr.AddCachedChecker(It.Is<TestingServer>(s => s.Ip == _testServer.Ip),
                It.Is<Checker>(c => c.Id == _checker.Id)), Times.Once);

            Assert.Equal(1, _context.CodeUploads.Count());
            var upload = _context.CodeUploads.First();
            Assert.Equal(UploadStatus.Testing, upload.Status);

            _transferMock.Verify(t => t.SendFile(It.IsNotNull<FileRequest>(), It.IsNotNull<byte[]>()), Times.Once);
        }
    }
}
