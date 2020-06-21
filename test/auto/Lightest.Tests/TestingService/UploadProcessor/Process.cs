using System;
using System.Linq;
using System.Threading.Tasks;
using GrpcServices;
using Lightest.Data.Models.TaskModels;
using Moq;
using Xunit;

namespace Lightest.Tests.TestingService.UploadProcessor
{
    public class Process : BaseTest
    {
        protected override Lightest.TestingService.DefaultServices.UploadProcessor _uploadProcessor =>
            _processorMock.Object;

        private readonly Mock<Lightest.TestingService.DefaultServices.UploadProcessor> _processorMock;
        private readonly TestingResponse _testingResponse;

        public Process()
        {
            _testingResponse = new TestingResponse
            {
                UploadId = _upload.Id.ToString()
            };

            _processorMock =
                new Mock<Lightest.TestingService.DefaultServices.UploadProcessor>(_upload, _uploadData, _server, _context);

            _processorMock.Setup(p => p.CacheChecker()).ThrowsAsync(new Exception());

            _processorMock.Setup(p => p.SendData()).ReturnsAsync(_testingResponse);

            _processorMock.Setup(p => p.ReportCodeResult(It.IsAny<TestingResponse>()))
                .ThrowsAsync(new Exception());
            _processorMock.Setup(p => p.ReportCodeResult(_testingResponse)).Returns(Task.CompletedTask);
        }

        [Fact]
        public async Task CheckerCompilationFailed()
        {
            _processorMock.Setup(p => p.CacheChecker()).ReturnsAsync(false);
            await SetUpData();

            await _uploadProcessor.Process();

            var upload = _context.Uploads.First(u => u.Id == _upload.Id);
            Assert.Equal(UploadStatus.Failed, upload.Status);
            Assert.Equal("Checker compilation failed", upload.Message);

            _processorMock.Verify(p => p.SendData(), Times.Never);
            _processorMock.Verify(p => p.ReportCodeResult(It.IsAny<TestingResponse>()), Times.Never);
        }

        [Fact]
        public async Task CheckerCompilationSucceeded()
        {
            _processorMock.Setup(p => p.CacheChecker()).ReturnsAsync(true);
            await SetUpData();

            await _uploadProcessor.Process();

            _processorMock.Verify(p => p.SendData(), Times.Once);
            _processorMock.Verify(p => p.ReportCodeResult(_testingResponse), Times.Once);
        }
    }
}
