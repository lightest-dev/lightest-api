using System;
using System.Threading.Tasks;
using Lightest.CodeManagment.Models;
using Lightest.Data.Models;
using Lightest.Data.Models.TaskModels;
using Lightest.TestingService.Interfaces;
using Lightest.TestingService.Models;
using Moq;

namespace Lightest.Tests.TestingService.UploadProcessor
{
    public abstract class BaseTest : TestingService.BaseTest
    {
        protected readonly Upload _upload;
        protected readonly TaskDefinition _task;
        protected readonly UploadData _uploadData;

        protected readonly TestingServer _server;
        protected readonly TestingServerInfo _serverInfo;
        protected readonly Mock<ITransferService> _transferService;

        protected virtual Lightest.TestingService.DefaultServices.UploadProcessor _uploadProcessor =>
            new Lightest.TestingService.DefaultServices.UploadProcessor(_upload, _uploadData, _server, _context);

        public BaseTest()
        {
            _transferService = new Mock<ITransferService>();
            _serverInfo = new TestingServerInfo
            {
                Ip = "1.1.1.1",
                Port = 443,
                Status = ServerStatus.Busy
            };

            _server = new TestingServer(_serverInfo, _transferService.Object);

            _task = new TaskDefinition
            {
                Id = Guid.NewGuid(),
                Points = 100
            };
            _upload = new Upload
            {
                Id = Guid.NewGuid(),
                TaskId = _task.Id,
                Task = _task,
                UserId = Guid.NewGuid().ToString()
            };
            _uploadData = new UploadData(_upload.Id, nameof(_uploadData));
        }

        protected virtual async Task SetUpData()
        {
            _context.Tasks.Add(_task);
            _context.Uploads.Add(_upload);
            _context.Servers.Add(_serverInfo);

            await _context.SaveChangesAsync();
        }
    }
}
