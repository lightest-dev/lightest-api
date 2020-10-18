using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Lightest.Data.Models;
using Lightest.Data.Models.TaskModels;
using Lightest.TestingService.Models;
using Lightest.Tests.Api.Tests.UploadsController;
using Moq;
using Xunit;

namespace Lightest.Tests.TestingService.TestingRunner
{
    public class TryStartNewWorker : BaseTest
    {
        private readonly TestingServerInfo _freeServerInfo;
        private readonly Upload _queuedUpload;

        private const string UploadCode = nameof(UploadCode);

        public TryStartNewWorker()
        {
            _freeServerInfo = new TestingServerInfo
            {
                Ip = "1.1.1.1",
                Port = 443,
                Status = ServerStatus.Free
            };

            _queuedUpload = new Upload
            {
                Id = Guid.NewGuid(),
                Status = UploadStatus.Queue
            };
        }

        protected async Task AddToDb()
        {
            _context.Uploads.Add(new Upload
            {
                Id = Guid.NewGuid(),
                Status = UploadStatus.New
            });
            _context.Uploads.Add(new Upload
            {
                Id = Guid.NewGuid(),
                Status = UploadStatus.Failed
            });
            _context.Uploads.Add(new Upload
            {
                Id = Guid.NewGuid(),
                Status = UploadStatus.Testing
            });
            _context.Uploads.Add(new Upload
            {
                Id = Guid.NewGuid(),
                Status = UploadStatus.EnvironmentSetup
            });
            _context.Uploads.Add(_queuedUpload);

            _context.Servers.Add(new TestingServerInfo
            {
                Ip = "1.1.1.2",
                Status = ServerStatus.Busy
            });
            _context.Servers.Add(new TestingServerInfo
            {
                Ip = "1.1.1.3",
                Status = ServerStatus.NotResponding
            });
            _context.Servers.Add(_freeServerInfo);

            await _context.SaveChangesAsync();
        }

        [Fact]
        public async Task NoFreeServer()
        {
            _freeServerInfo.Status = ServerStatus.Busy;
            await AddToDb();

            await _testingRunner.TryStartNewWorker();

            _transferServiceFactory.Verify(f => f.Create(It.IsAny<IPAddress>(), It.IsAny<int>()), Times.Never);
        }

        [Fact]
        public async Task NoQueuedUpload()
        {
            _queuedUpload.Status = UploadStatus.Failed;
            await AddToDb();

            await _testingRunner.TryStartNewWorker();

            _codeManagmentService.Verify(c => c.Get(It.IsAny<Guid>()), Times.Never);
            var server = _context.Servers.First(s => s.Ip == _freeServerInfo.Ip);
            Assert.Equal(ServerStatus.Free, server.Status);
        }

        [Fact]
        public async Task MultipleItemsProcessed()
        {
            var uploads = new[]
            {
                _queuedUpload,
                new Upload
                {
                    Id = Guid.NewGuid(),
                    Status = UploadStatus.Queue
                },
                new Upload
                {
                    Id = Guid.NewGuid(),
                    Status = UploadStatus.Queue
                }
            };

            var language = new Language
            {
                Id = Guid.NewGuid()
            };

            _context.Languages.Add(language);

            var data = new CodeManagment.Models.UploadData[3];

            _context.Uploads.Add(uploads[1]);
            _context.Uploads.Add(uploads[2]);

            for(var i = 0; i < uploads.Length; i++)
            {
                var upload = uploads[i];
                var currentData = new CodeManagment.Models.UploadData
                {
                    Id = upload.Id,
                    Code = $"{UploadCode}{i}"
                };
                data[i] = currentData;
                _codeManagmentService.Setup(cm => cm.Get(upload.Id))
                    .Returns(currentData);
            }

            await AddToDb();

            foreach (var upload in _context.Uploads)
            {
                upload.Task = new TaskDefinition
                {
                    Checker = new Checker(),
                    Languages = new List<TaskLanguage>(),
                    Tests = new List<Test>(),
                };

                upload.Language = language;
            }
            await _context.SaveChangesAsync();

            await _testingRunner.TryStartNewWorker();

            for (var i = 0; i < uploads.Length; i++)
            {
                var upload = _context.Uploads.First(u => u.Id == uploads[i].Id);
                var currentData = data[i];

                Assert.Equal(UploadStatus.EnvironmentSetup, upload.Status);
                _uploadProcessorFactory.Verify(f => f.Create(
                    It.Is<Upload>(u => u.Id == upload.Id),
                    currentData,
                    It.Is<TestingServer>(s => s.TransferService == _transferService.Object
                        && s.ServerInfo.Ip == _freeServerInfo.Ip),
                    _context), Times.Once);
            }

            _uploadProcessor.Verify(p => p.Process(), Times.Exactly(uploads.Length));
            var server = _context.Servers.First(s => s.Ip == _freeServerInfo.Ip);
            Assert.Equal(ServerStatus.Free, server.Status);
        }
    }
}
