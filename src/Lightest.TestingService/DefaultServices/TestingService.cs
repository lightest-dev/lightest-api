using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Lightest.Data;
using Lightest.Data.Models;
using Lightest.Data.Models.TaskModels;
using Lightest.TestingService.Interfaces;
using Lightest.TestingService.Models;
using Lightest.TestingService.Requests;
using Microsoft.EntityFrameworkCore;

namespace Lightest.TestingService.DefaultServices
{
    public class TestingService : ITestingService
    {
        private readonly RelationalDbContext _context;
        private readonly IServerRepository _repository;
        private readonly ITransferServiceFactory _transferServiceFactory;

        public TestingService(IServerRepository repository, RelationalDbContext context, ITransferServiceFactory transferServiceFactory)
        {
            _context = context;
            _repository = repository;
            _transferServiceFactory = transferServiceFactory;
        }

        public async Task<bool> BeginTesting(IUpload upload)
        {
            await AddToList(upload);

            var server = _repository.GetFreeServer();
            if (server == null)
            {
                return false;
            }

            var transferService = _transferServiceFactory.Create(server.IPAddress, server.Port);

            var result = await CacheChecker(server, upload.Task.Checker, transferService);

            if (!result)
            {
                _repository.AddBrokenServer(server);
                return false;
            }

            switch (upload)
            {
                case CodeUpload code:
                    result = await SendData(code, transferService);
                    break;

                default:
                    throw new NotImplementedException();
            }

            if (result)
            {
                upload.Status = UploadStatus.Testing;
                await _context.SaveChangesAsync();
            }
            else
            {
                _repository.AddBrokenServer(server);
            }
            return result;
        }

        public Task ReportResult(CheckerResult result)
        {
            var server = new TestingServer
            {
                Ip = result.Ip
            };
            _repository.AddFreeServer(server);

            if (result.Type.ToLower() == "code")
            {
                return ReportCodeResult(result);
            }

            throw new NotImplementedException($"Uploads with type {result.Type} are currently not supported");
        }

        public Task StartNextTesting()
        {
            var upload = _context.CodeUploads.FirstOrDefault(c => c.Status == UploadStatus.Queue);

            //todo: refactor to enable complete testing, maybe move BeginTesting to separate class
            if (upload != null)
            {
                return BeginTesting(upload);
            }

            return Task.CompletedTask;
        }

        private async Task ReportCodeResult(CheckerResult result)
        {
            var upload = await _context.CodeUploads
                .SingleOrDefaultAsync(u => u.UploadId == result.UploadId);
            var userTask = await _context.UserTasks
                .SingleOrDefaultAsync(u => u.UserId == upload.UserId
                && u.TaskId == upload.TaskId);
            var totalTests = await _context.Tests
                .Where(t => t.TaskId == upload.TaskId)
                .CountAsync();
            var maxPoints = _context.Tasks
                .Where(t => t.Id == upload.TaskId)
                .Select(t => t.Points)
                .First();
            var successfulPercent = (double)result.SuccessfulTests / totalTests;
            upload.Points = successfulPercent * maxPoints;
            upload.Status = result.Status;
            upload.Message = result.Message;
            if (userTask?.HighScore < upload.Points)
            {
                userTask.HighScore = upload.Points;
                if (result.SuccessfulTests == totalTests)
                {
                    userTask.Completed = true;
                }
            }
            await _context.SaveChangesAsync();
        }

        private async Task AddToList(IUpload upload)
        {
            upload.Status = UploadStatus.Queue;
            switch (upload)
            {
                case CodeUpload code:
                    await StartTrackingCodeUpload(code);
                    break;

                default:
                    throw new NotImplementedException();
            }
        }

        private async Task<bool> SendData(CodeUpload upload, ITransferService transferService)
        {
            upload.Status = UploadStatus.Queue;
            var save = _context.SaveChangesAsync();
            var language = upload.Task.Languages.FirstOrDefault(l => l.LanguageId == upload.LanguageId);
            var request = new TestingRequest
            {
                UploadId = upload.UploadId,
                MemoryLimit = language.MemoryLimit,
                TimeLimit = language.TimeLimit,
                CheckerId = upload.Task.CheckerId,
                TestsCount = upload.Task.Tests.Count
            };
            var result = await transferService.SendMessage(request.ToString());
            if (!result)
            {
                return false;
            }
            var i = 0;

            FileRequest fileRequest;
            foreach (var test in upload.Task.Tests)
            {
                fileRequest = new TestRequest($"{i}.in");
                result = await transferService.SendFile(fileRequest, Encoding.UTF8.GetBytes(test.Input));
                if (!result)
                {
                    return false;
                }
                fileRequest = new TestRequest($"{i}.out");
                result = await transferService.SendFile(fileRequest, Encoding.UTF8.GetBytes(test.Output));
                if (!result)
                {
                    return false;
                }
                i++;
            }
            await save;
            fileRequest = new SingleFileCodeRequest($"{upload.UploadId}.{upload.Language.Extension}");
            result = await transferService.SendFile(fileRequest, Encoding.UTF8.GetBytes(upload.Code));
            return result;
        }

        private async Task<bool> SendChecker(Checker checker, ITransferService transferService)
        {
            var checkerRequest = new CheckerRequest
            {
                Id = checker.Id.ToString(),
                Code = checker.Code
            };
            var result = await transferService.SendMessage(checkerRequest.ToString());
            return result;
        }

        private async Task<bool> CacheChecker(TestingServer server, Checker checker, ITransferService transferService)
        {
            var result = true;
            if (!_context.CachedCheckers.Any(c => c.CheckerId == checker.Id && c.ServerIp == server.Ip))
            {
                result = await SendChecker(checker, transferService);
                if (result)
                {
                    _repository.AddCachedChecker(server, checker);
                }
            }
            return result;
        }

        //todo: rename
        private async Task StartTrackingCodeUpload(CodeUpload upload)
        {
            if (!_context.CodeUploads.Any(u => u.UploadId == upload.UploadId))
            {
                _context.CodeUploads.Add(upload);
                await _context.SaveChangesAsync();
            }
        }

        public Task ReportNewServer(NewServer serverData)
        {
            var server = new TestingServer
            {
                Ip = serverData.Ip,
                Version = serverData.ServerVersion,
                Port = 10000,
                Status = ServerStatus.Free
            };
            _repository.AddNewServer(server);
            return Task.CompletedTask;
        }

        public Task ReportBrokenServer(NewServer serverData)
        {
            var server = new TestingServer
            {
                Ip = serverData.Ip
            };
            _repository.AddBrokenServer(server);
            return Task.CompletedTask;
        }
    }
}
