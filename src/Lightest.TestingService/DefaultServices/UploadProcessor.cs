using System;
using System.Linq;
using System.Threading.Tasks;
using GrpcServices;
using Lightest.CodeManagment.Models;
using Lightest.Data;
using Lightest.Data.Models;
using Lightest.Data.Models.TaskModels;
using Lightest.TestingService.Interfaces;
using Lightest.TestingService.Models;
using Microsoft.EntityFrameworkCore;

namespace Lightest.TestingService.DefaultServices
{
    public class UploadProcessor : IUploadProcessor
    {
        private readonly Upload _upload;
        private readonly UploadData _uploadData;
        private readonly TestingServer _server;
        private readonly RelationalDbContext _context;

        public UploadProcessor(Upload upload, UploadData uploadData,
            TestingServer server, RelationalDbContext context)
        {
            _upload = upload;
            _uploadData = uploadData;
            _server = server;
            _context = context;
        }

        private async Task<bool> SendChecker(Checker checker)
        {
            var checkerRequest = new CheckerRequest
            {
                Id = checker.Id.ToString(),
                Code = checker.Code
            };
            var result = await _server.TransferService.SendChecker(checkerRequest);

            checker.Compiled = result.Compiled;
            checker.Message = result.Message;
            await _context.SaveChangesAsync();

            return result.Compiled;
        }

        public async Task<bool> CacheChecker()
        {
            var result = true;
            var checker = _upload.Task.Checker;
            if (!_context.CachedCheckers.Any(c => c.CheckerId == checker.Id
                && c.ServerIp == _server.ServerInfo.Ip))
            {
                result = await SendChecker(checker);
                if (result)
                {
                    var cachedChecker = new ServerChecker
                    {
                        ServerIp = _server.ServerInfo.Ip,
                        CheckerId = checker.Id
                    };
                    _context.CachedCheckers.Add(cachedChecker);
                    await _context.SaveChangesAsync();
                }
            }
            return result;
        }

        public async Task<TestingResponse> SendData()
        {
            _upload.Status = UploadStatus.Testing;
            await _context.SaveChangesAsync();
            var language = _upload.Task.Languages.FirstOrDefault(l => l.LanguageId == _upload.LanguageId);
            var request = new TestingRequest
            {
                UploadId = _upload.Id.ToString(),
                MemoryLimit = language.MemoryLimit,
                TimeLimit = language.TimeLimit,
                CheckerId = _upload.Task.CheckerId.ToString(),
                CodeFile = new CodeFile
                {
                    FileName = $"{_upload.Id.ToString()}.{_upload.Language.Extension}",
                    Code = _uploadData.Code
                }
            };

            request.Tests.AddRange(_upload.Task.Tests.Select((t, i) => new GrpcServices.Test
            {
                TestName = i.ToString(),
                Input = t.Input,
                Output = t.Output
            }));

            return await _server.TransferService.SendUpload(request);
        }

        public async Task ReportCodeResult(TestingResponse result)
        {
            var uploadId = Guid.Parse(result.UploadId);
            var userTask = await _context.UserTasks
                .SingleOrDefaultAsync(u => u.UserId == _upload.UserId
                && u.TaskId == _upload.TaskId);
            var totalTests = _upload.Task.Tests.Count;
            var maxPoints = _upload.Task.Points;
            var successfulPercent = (double)result.SuccessfulTestsCount / totalTests;
            _upload.Points = successfulPercent * maxPoints;
            _upload.Status = result.Status;
            _upload.Message = result.Message;
            if (userTask?.HighScore < _upload.Points)
            {
                userTask.HighScore = _upload.Points;
                if (result.SuccessfulTestsCount == totalTests)
                {
                    userTask.Completed = true;
                }
            }
            await _context.SaveChangesAsync();
        }
    }
}
