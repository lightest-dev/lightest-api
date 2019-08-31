using System;
using System.Linq;
using System.Threading.Tasks;
using Lightest.Api.ResponseModels;
using Lightest.Data.Models;
using Lightest.Data.Models.TaskModels;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;

namespace Lightest.Tests.Api.Tests.UploadsController
{
    public class GetResult : BaseTest
    {
        private readonly Upload _upload;

        public GetResult()
        {
            _upload = GenerateUploads(1).First();
            _upload.Status = "status";
            _upload.Message = "message";
            _upload.Points = 11;
        }

        [Fact]
        public async Task Found()
        {
            _context.Tasks.Add(_task);
            _context.Uploads.Add(_upload);
            await _context.SaveChangesAsync();

            var result = await _controller.GetResult(_upload.Id);
            var okResult = result as OkObjectResult;
            Assert.NotNull(okResult);

            var upload = okResult.Value as UserUploadResult;
            Assert.NotNull(upload);

            Assert.Equal(_upload.Id, upload.Id);
            Assert.Equal(_upload.Message, upload.Message);
            Assert.Equal(_upload.Points, upload.Points);
            Assert.Equal(_upload.Status, upload.Status);
        }

        [Fact]
        public async Task Forbidden()
        {
            _context.Tasks.Add(_task);
            _context.Uploads.Add(_upload);
            await _context.SaveChangesAsync();

            _accessServiceMock.Setup(m => m.CheckReadAccess(It.IsAny<Upload>(),
                It.Is<ApplicationUser>(u => u.Id == _user.Id)))
                .Returns(false);

            var result = await _controller.GetResult(_upload.Id);

            Assert.IsAssignableFrom<ForbidResult>(result);
        }

        [Fact]
        public async Task NotFound()
        {
            _context.Tasks.Add(_task);
            await _context.SaveChangesAsync();

            var result = await _controller.GetResult(Guid.NewGuid());

            Assert.IsAssignableFrom<NotFoundResult>(result);
        }
    }
}
