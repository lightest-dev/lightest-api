using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Lightest.Api.RequestModels.UploadRequests;
using Lightest.Data.Models;
using Lightest.Data.Models.TaskModels;
using Lightest.Data.Mongo.Models;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;

namespace Lightest.Tests.Api.Tests.UploadsController
{
    public class UploadCode : BaseTest
    {
        private  CodeUpload _codeUpload;
        private readonly Language _language;

        public UploadCode()
        {
            _language = new Language
            {
                Id = Guid.NewGuid(),
                Name = "name",
                Extension = ".ext"
            };
            GenerateCodeUploads();
        }

        internal void GenerateCodeUploads()
        {
            var codeUpload = new CodeUpload
            {
                Id = Guid.NewGuid(),
                Code = "code",
                TaskId = _task.Id,
                LanguageId = _language.Id
            };
            _codeUpload = codeUpload;
        }

        [Fact]
        public async Task TaskNotFound()
        {
            var result = await _controller.UploadCode(_codeUpload);
            var badRequestResult = result as BadRequestObjectResult;
            Assert.NotNull(badRequestResult);

            var errors = badRequestResult.Value as SerializableError;
            Assert.NotNull(errors);

            Assert.Contains(nameof(_codeUpload.TaskId), errors.Keys);
        }

        [Fact]
        public async Task Forbidden()
        {
            _context.Tasks.Add(_task);
            await _context.SaveChangesAsync();

            _accessServiceMock.Setup(m => m.CanAdd(It.IsAny<Upload>(),
                It.Is<ApplicationUser>(u => u.Id == _user.Id)))
                .ReturnsAsync(false);

            var result = await _controller.UploadCode(_codeUpload);

            Assert.IsAssignableFrom<ForbidResult>(result);
        }

        [Fact]
        public async Task LanguageNotFound()
        {
            _codeUpload.LanguageId = Guid.NewGuid();
            _context.Tasks.Add(_task);
            await _context.SaveChangesAsync();

            var result = await _controller.UploadCode(_codeUpload);
            var badRequestResult = result as BadRequestObjectResult;
            Assert.NotNull(badRequestResult);

            var errors = badRequestResult.Value as SerializableError;
            Assert.NotNull(errors);

            Assert.Contains(nameof(_codeUpload.LanguageId), errors.Keys);
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
            _codeUpload.LanguageId = language.Id;
            _context.Languages.Add(language);
            _context.Tasks.Add(_task);
            await _context.SaveChangesAsync();

            var result = await _controller.UploadCode(_codeUpload);
            var badRequestResult = result as BadRequestObjectResult;
            Assert.NotNull(badRequestResult);

            var errors = badRequestResult.Value as SerializableError;
            Assert.NotNull(errors);

            Assert.Contains(nameof(_codeUpload.LanguageId), errors.Keys);
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
            _codeUpload.LanguageId = language.Id;
            _context.Languages.Add(language);
            _context.Tasks.Add(_task);
            await _context.SaveChangesAsync();

            var result = await _controller.UploadCode(_codeUpload);
            var okResult = result as OkObjectResult;
            Assert.NotNull(okResult);
            var id = (Guid)okResult.Value;
            Assert.Single(_context.Uploads); 
            var upload = _context.Uploads.Find(id);
            Assert.Equal(UploadStatus.New, upload.Status);
            Assert.Equal(0, upload.Points);
            Assert.Equal(_user.Id, upload.UserId);
            Assert.Equal(DateTime.Now.Date, upload.UploadDate.Date);

            _testingServiceMock.Verify(m => m.BeginTesting(It.Is<Upload>(u => u.Id == id), It.Is<UploadData>(u => u.Id == id)), Times.Once);
        }

        [Fact]
        public async Task UploadEqualUploadData()
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
            _codeUpload.LanguageId = language.Id;
            _context.Languages.Add(language);
            _context.Tasks.Add(_task);
            await _context.SaveChangesAsync();

            await _controller.UploadCode(_codeUpload);
            _testingServiceMock.Verify(m => m.BeginTesting(It.IsAny<Upload>(), It.Is<UploadData>(u => u.Code == _codeUpload.Code)), Times.Once);
            _testingServiceMock.Verify(m => m.BeginTesting(It.Is<Upload>(u => u.LanguageId == _codeUpload.LanguageId), It.IsAny<UploadData>()), Times.Once);
            _testingServiceMock.Verify(m => m.BeginTesting(It.Is<Upload>(u => u.TaskId == _codeUpload.TaskId), It.IsAny<UploadData>()), Times.Once);

        }
    }
}
