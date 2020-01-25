using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Lightest.Data.Models;
using Lightest.Data.Models.TaskModels;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;

namespace Lightest.Tests.Api.Tests.UploadsController
{
    public class UploadCode : BaseTest
    {
        private readonly Upload _upload;

        public UploadCode()
        {
            _upload = GenerateUploads(1).First();
            _upload.Points = 5;
            _upload.UserId = Guid.NewGuid().ToString();
            _upload.UploadDate = DateTime.Now.AddDays(-10);
        }

        [Fact]
        public async Task TaskNotFound()
        {
            var result = await _controller.UploadCode(_upload);
            var badRequestResult = result as BadRequestObjectResult;
            Assert.NotNull(badRequestResult);

            var errors = badRequestResult.Value as SerializableError;
            Assert.NotNull(errors);

            Assert.Contains(nameof(_upload.TaskId), errors.Keys);
        }

        [Fact]
        public async Task Forbidden()
        {
            _context.Tasks.Add(_task);
            await _context.SaveChangesAsync();

            _accessServiceMock.Setup(m => m.HasWriteAccess(It.IsAny<Guid>(),
                It.Is<ApplicationUser>(u => u.Id == _user.Id)))
                .ReturnsAsync(false);

            var result = await _controller.UploadCode(_upload);

            Assert.IsAssignableFrom<ForbidResult>(result);
        }

        [Fact]
        public async Task LanguageNotFound()
        {
            _upload.LanguageId = Guid.NewGuid();
            _context.Tasks.Add(_task);
            await _context.SaveChangesAsync();

            var result = await _controller.UploadCode(_upload);
            var badRequestResult = result as BadRequestObjectResult;
            Assert.NotNull(badRequestResult);

            var errors = badRequestResult.Value as SerializableError;
            Assert.NotNull(errors);

            Assert.Contains(nameof(_upload.LanguageId), errors.Keys);
        }

        [Fact]
        public async Task LanguageNotAssignedToTask()
        {
            var language = new Language
            {
                Name = "name",
                Extension = "extension",
                Id = Guid.NewGuid()
            };
            _upload.LanguageId = language.Id;
            _context.Languages.Add(language);
            _context.Tasks.Add(_task);
            await _context.SaveChangesAsync();

            var result = await _controller.UploadCode(_upload);
            var badRequestResult = result as BadRequestObjectResult;
            Assert.NotNull(badRequestResult);

            var errors = badRequestResult.Value as SerializableError;
            Assert.NotNull(errors);

            Assert.Contains(nameof(_upload.LanguageId), errors.Keys);
        }

        [Fact]
        public async Task Added()
        {
            var language = new Language
            {
                Name = "name",
                Extension = "extension",
                Id = Guid.NewGuid()
            };
            _task.Languages = new List<TaskLanguage>
            {
                new TaskLanguage
                {
                    Language = language,
                    LanguageId = language.Id,
                    Task = _task,
                    TaskId = _task.Id
                }
            };
            _upload.LanguageId = language.Id;
            _context.Languages.Add(language);
            _context.Tasks.Add(_task);
            await _context.SaveChangesAsync();

            var result = await _controller.UploadCode(_upload);

            var okResult = result as OkObjectResult;
            Assert.NotNull(okResult);

            var id = (Guid)okResult.Value;
            Assert.Equal(_upload.Id, id);

            Assert.Single(_context.Uploads);
            var upload = _context.Uploads.First();
            Assert.Equal(_upload.Id, upload.Id);
            Assert.Equal(UploadStatus.New, upload.Status);
            Assert.Equal(0, upload.Points);
            Assert.Equal(_user.Id, upload.UserId);
            Assert.Equal(DateTime.Now.Date, upload.UploadDate.Date);

            _testingServiceMock.Verify(m => m.BeginTesting(It.Is<Upload>(u => u.Id == _upload.Id)), Times.Once);
        }
    }
}
