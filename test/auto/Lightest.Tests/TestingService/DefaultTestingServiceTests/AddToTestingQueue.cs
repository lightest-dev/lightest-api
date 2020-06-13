using System;
using System.Threading.Tasks;
using Lightest.Data.Models.TaskModels;
using Microsoft.EntityFrameworkCore;
using Moq;
using Xunit;

namespace Lightest.Tests.TestingService.DefaultTestingServiceTests
{
    public class AddToTestingQueue : BaseTests
    {
        private readonly Upload _upload;

        public AddToTestingQueue()
        {
            _upload = new Upload
            {
                Id = Guid.NewGuid()
            };
        }

        private void AddToDb()
        {
            _context.Uploads.Add(_upload);
            _context.SaveChanges();
        }

        [Fact]
        public async Task UploadAdded()
        {
            AddToDb();

            await _testingService.AddToTestingQueue(_upload);

            var upload = await _context.Uploads.FirstOrDefaultAsync(u => u.Id == _upload.Id);
            Assert.NotNull(upload);
            Assert.Equal(UploadStatus.Queue, upload.Status);

            _testingRunner.Verify(r => r.TryStartNewWorker(), Times.Once);
        }
    }
}
