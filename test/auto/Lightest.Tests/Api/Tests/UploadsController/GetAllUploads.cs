using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Lightest.Api.ResponseModels.UploadViews;
using Lightest.Data.Models;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;

namespace Lightest.Tests.Api.Tests.UploadsController
{
    public class GetAllUploads : BaseTest
    {
        [Fact]
        public async Task VerifyUnlimitedAndSorted()
        {
            var uploads = GenerateUploads(15);
            _context.Tasks.Add(_task);
            _context.Uploads.AddRange(uploads);
            uploads = uploads.OrderByDescending(u => u.UploadDate).ToList();
            await _context.SaveChangesAsync();

            var result = await _controller.GetAllUploads(_task.Id);
            var okResult = result as OkObjectResult;
            Assert.NotNull(okResult);

            var uploadsResult = (okResult.Value as IEnumerable<LastUploadView>).ToList();
            Assert.Equal(15, uploadsResult.Count());
            for (var i = 0; i < 15; i++)
            {
                var upload = uploads[i];
                var resultUpload = uploadsResult[i];
                Assert.Equal(upload.Id, resultUpload.Id);
            }
        }

        [Fact]
        public async Task VerifyUploadsFromAllUsersReturned()
        {
            var uploads = GenerateUploads(10);
            uploads[0].UserId = Guid.NewGuid().ToString();
            uploads[1].UserId = Guid.NewGuid().ToString();
            _context.Tasks.Add(_task);
            _context.Uploads.AddRange(uploads);
            await _context.SaveChangesAsync();

            var result = await _controller.GetAllUploads(_task.Id);
            var okResult = result as OkObjectResult;
            Assert.NotNull(okResult);

            var uploadsResult = okResult.Value as IEnumerable<LastUploadView>;
            Assert.NotNull(uploadsResult);

            Assert.Equal(10, uploadsResult.Count());
        }

        [Fact]
        public async Task Forbidden()
        {
            _context.Tasks.Add(_task);
            await _context.SaveChangesAsync();

            _accessServiceMock.Setup(m => m.CanWrite(It.IsAny<Guid>(),
                It.Is<ApplicationUser>(u => u.Id == _user.Id)))
                .ReturnsAsync(false);

            var result = await _controller.GetAllUploads(Guid.NewGuid());

            Assert.IsAssignableFrom<ForbidResult>(result);
        }
    }
}
