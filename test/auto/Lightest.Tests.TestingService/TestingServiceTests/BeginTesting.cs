using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Lightest.Data.Models;
using Lightest.Data.Models.TaskModels;
using Lightest.TestingService.Requests;
using Moq;
using Xunit;

namespace Lightest.Tests.TestingService.TestingServiceTests
{
    public class BeginTesting : BaseTests
    {
        private readonly TestingServer _testServer;

        private readonly Checker _checker;

        private readonly Language _language;

        private readonly TaskDefinition _task;

        private readonly CodeUpload _upload;

        private readonly Test _test;

        public BeginTesting()
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

            _test = new Test
            {
                Input = "input",
                Output = "output"
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

            _task.Tests.Add(_test);

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
        public async Task NoFreeServer()
        {
            _serverRepoMock.Setup(r => r.GetFreeServer())
                .Returns(default(TestingServer));

            var result = await _testingService.BeginTesting(_upload);

            Assert.False(result);

            _serverRepoMock.Verify(s => s.GetFreeServer(), Times.Once);

            _transferMock.VerifyNoOtherCalls();
            _serverRepoMock.VerifyNoOtherCalls();
            _factoryMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task CachingFailed()
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
        public async Task UploadSendingFailed()
        {
            _transferMock.Setup(t => t.SendFile(It.IsNotNull<FileRequest>(), 
                It.Is<byte[]>(b => b.SequenceEqual(Encoding.UTF8.GetBytes(_upload.Code)))))
                .Returns(Task.FromResult(false));

            var result = await _testingService.BeginTesting(_upload);

            Assert.False(result);
            _serverRepoMock.Verify(s => s.GetFreeServer(), Times.Once);
            _transferMock.Verify(t => t.SendMessage(It.IsNotNull<string>()), Times.Exactly(2));

            _transferMock.Verify(t => t.SendFile(
                It.IsNotNull<FileRequest>(),
                It.Is<byte[]>(b => b.SequenceEqual(Encoding.UTF8.GetBytes(_upload.Code)))),
                Times.Once);
            _transferMock.Verify(t => t.SendFile(
                It.IsNotNull<FileRequest>(),
                It.Is<byte[]>(b => b.SequenceEqual(Encoding.UTF8.GetBytes(_test.Input)))),
                Times.AtMostOnce);
            _transferMock.Verify(t => t.SendFile(
                It.IsNotNull<FileRequest>(),
                It.Is<byte[]>(b => b.SequenceEqual(Encoding.UTF8.GetBytes(_test.Output)))),
                Times.AtMostOnce);

            _serverRepoMock.Verify(sr => sr.AddBrokenServer(It.Is<TestingServer>(s => s.Ip == _testServer.Ip)), Times.Once);
            _serverRepoMock.Verify(sr => sr.AddCachedChecker(It.Is<TestingServer>(s => s.Ip == _testServer.Ip),
                It.Is<Checker>(c => c.Id == _checker.Id)), Times.Once);

            Assert.Equal(1, _context.CodeUploads.Count());
            var upload = _context.CodeUploads.First();
            Assert.Equal(UploadStatus.Queue, upload.Status);

            _transferMock.VerifyNoOtherCalls();
            _serverRepoMock.VerifyNoOtherCalls();
            _factoryMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task TestInputSendingFailed()
        {
            _transferMock.Setup(t => t.SendFile(It.IsNotNull<FileRequest>(), 
                It.Is<byte[]>(b => b.SequenceEqual(Encoding.UTF8.GetBytes(_test.Input)))))
                .Returns(Task.FromResult(false));

            var result = await _testingService.BeginTesting(_upload);

            Assert.False(result);
            _serverRepoMock.Verify(s => s.GetFreeServer(), Times.Once);
            _transferMock.Verify(t => t.SendMessage(It.IsNotNull<string>()), Times.Exactly(2));

            _transferMock.Verify(t => t.SendFile(
                It.IsNotNull<FileRequest>(),
                It.Is<byte[]>(b => b.SequenceEqual(Encoding.UTF8.GetBytes(_upload.Code)))),
                Times.Never);
            _transferMock.Verify(t => t.SendFile(
                It.IsNotNull<FileRequest>(),
                It.Is<byte[]>(b => b.SequenceEqual(Encoding.UTF8.GetBytes(_test.Input)))), 
                Times.AtMostOnce);
            _transferMock.Verify(t => t.SendFile(
                It.IsNotNull<FileRequest>(),
                It.Is<byte[]>(b => b.SequenceEqual(Encoding.UTF8.GetBytes(_test.Output)))),
                Times.AtMostOnce);

            _serverRepoMock.Verify(sr => sr.AddBrokenServer(It.Is<TestingServer>(s => s.Ip == _testServer.Ip)), Times.Once);
            _serverRepoMock.Verify(sr => sr.AddCachedChecker(It.Is<TestingServer>(s => s.Ip == _testServer.Ip),
                It.Is<Checker>(c => c.Id == _checker.Id)), Times.Once);

            Assert.Equal(1, _context.CodeUploads.Count());
            var upload = _context.CodeUploads.First();
            Assert.Equal(UploadStatus.Queue, upload.Status);

            _transferMock.VerifyNoOtherCalls();
            _serverRepoMock.VerifyNoOtherCalls();
            _factoryMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task TestOutputSendingFailed()
        {
            _transferMock.Setup(t => t.SendFile(It.IsNotNull<FileRequest>(), 
                It.Is<byte[]>(b => b.SequenceEqual(Encoding.UTF8.GetBytes(_test.Output)))))
                .Returns(Task.FromResult(false));

            var result = await _testingService.BeginTesting(_upload);

            Assert.False(result);
            _serverRepoMock.Verify(s => s.GetFreeServer(), Times.Once);
            _transferMock.Verify(t => t.SendMessage(It.IsNotNull<string>()), Times.Exactly(2));

            _transferMock.Verify(t => t.SendFile(
                It.IsNotNull<FileRequest>(),
                It.Is<byte[]>(b => b.SequenceEqual(Encoding.UTF8.GetBytes(_test.Input)))),
                Times.AtMostOnce);
            _transferMock.Verify(t => t.SendFile(
                It.IsNotNull<FileRequest>(),
                It.Is<byte[]>(b => b.SequenceEqual(Encoding.UTF8.GetBytes(_test.Output)))),
                Times.Once);

            _serverRepoMock.Verify(sr => sr.AddBrokenServer(It.Is<TestingServer>(s => s.Ip == _testServer.Ip)), Times.Once);
            _serverRepoMock.Verify(sr => sr.AddCachedChecker(It.Is<TestingServer>(s => s.Ip == _testServer.Ip),
                It.Is<Checker>(c => c.Id == _checker.Id)), Times.Once);

            Assert.Equal(1, _context.CodeUploads.Count());
            var upload = _context.CodeUploads.First();
            Assert.Equal(UploadStatus.Queue, upload.Status);
        }

        [Fact]
        public async Task CachedChecker()
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
        }

        [Fact]
        public async Task NonCachedChecker()
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
        }
    }
}
