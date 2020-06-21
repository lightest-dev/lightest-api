using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GrpcServices;
using Lightest.Data.Models.TaskModels;
using Moq;
using Xunit;

namespace Lightest.Tests.TestingService.UploadProcessor
{
    public class SendData : BaseTest
    {
        private readonly Data.Models.TaskModels.Test _test1;
        private readonly Data.Models.TaskModels.Test _test2;
        private readonly Language _language;
        private readonly TaskLanguage _taskLanguage;

        public SendData()
        {
            _test1 = new Data.Models.TaskModels.Test
            {
                Input = $"{nameof(_test1)}in",
                Output = $"{nameof(_test1)}out",
                Id = Guid.NewGuid(),
                TaskId = _task.Id
            };
            _test2 = new Data.Models.TaskModels.Test
            {
                Input = $"{nameof(_test2)}in",
                Output = $"{nameof(_test2)}out",
                Id = Guid.NewGuid(),
                TaskId = _task.Id
            };
            _task.Tests = new List<Data.Models.TaskModels.Test>
            {
                _test1,
                _test2
            };

            _task.CheckerId = Guid.NewGuid();

            _language = new Language
            {
                Id = Guid.NewGuid(),
                Extension = "ext"
            };
            _upload.Language = _language;

            _taskLanguage = new TaskLanguage
            {
                TaskId = _task.Id,
                LanguageId = _language.Id,
                MemoryLimit = 256,
                TimeLimit = 1
            };
            _task.Languages = new List<TaskLanguage>
            {
                _taskLanguage
            };
        }

        protected override Task SetUpData()
        {
            _context.Tests.Add(_test1);
            _context.Tests.Add(_test2);
            _context.Languages.Add(_language);

            return base.SetUpData();
        }

        [Fact]
        public async Task UploadSent()
        {
            var response = new TestingResponse
            {
                UploadId = _upload.Id.ToString()
            };
            _transferService.Setup(ts => ts.SendUpload(It.Is<TestingRequest>(
                tr => tr.UploadId == _upload.Id.ToString() && tr.TimeLimit == _taskLanguage.TimeLimit &&
                tr.MemoryLimit == _taskLanguage.MemoryLimit && tr.CheckerId == _task.CheckerId.ToString() &&
                tr.CodeFile.FileName.EndsWith(_language.Extension) && tr.CodeFile.Code == _uploadData.Code &&
                tr.Tests.Count == 2)))
                .ReturnsAsync(response);
            await SetUpData();

            var result = await _uploadProcessor.SendData();

            var upload = _context.Uploads.First(u => u.Id == _upload.Id);
            Assert.Equal(UploadStatus.Testing, upload.Status);
            Assert.Equal(response, result);
        }
    }
}
