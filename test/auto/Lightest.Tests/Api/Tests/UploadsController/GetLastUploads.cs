using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Lightest.Api.ResponseModels;
using Microsoft.AspNetCore.Mvc;
using Xunit;

namespace Lightest.Tests.Api.Tests.UploadsController
{
    public class GetLastUploads : BaseTest
    {
        [Fact]
        public async Task VerifyLimitedAndSorted()
        {
            var uploads = GenerateUploads(15);
            _context.Tasks.Add(_task);
            _context.Uploads.AddRange(uploads);
            uploads = uploads.OrderByDescending(u => u.UploadDate).ToList();
            await _context.SaveChangesAsync();

            var result = await _controller.GetLastUploads(_task.Id);
            var okResult = result as OkObjectResult;
            Assert.NotNull(okResult);

            var uploadsResult = (okResult.Value as IEnumerable<LastUploadModel>).ToList();
            Assert.Equal(10, uploadsResult.Count());
            for (var i = 0; i < 10; i++)
            {
                var upload = uploads[i];
                var resultUpload = uploadsResult[i];
                Assert.Equal(upload.Id, resultUpload.Id);
            }
        }

        [Fact]
        public async Task VerifyOnlyUploadsFromCurrentUserReturned()
        {
            var uploads = GenerateUploads(10);
            uploads[0].UserId = Guid.NewGuid().ToString();
            uploads[1].UserId = Guid.NewGuid().ToString();
            _context.Tasks.Add(_task);
            _context.Uploads.AddRange(uploads);
            await _context.SaveChangesAsync();

            var result = await _controller.GetLastUploads(_task.Id);
            var okResult = result as OkObjectResult;
            Assert.NotNull(okResult);

            var uploadsResult = okResult.Value as IEnumerable<LastUploadModel>;
            Assert.NotNull(uploadsResult);

            Assert.Equal(8, uploadsResult.Count());
        }
    }
}
