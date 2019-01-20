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
        private readonly object _lock;
        private readonly IServerRepository _repository;
        private readonly List<IUpload> _uploads;
        private readonly ITransferServiceFactory _transferServiceFactory;

        public TestingService(IServerRepository repository, RelationalDbContext context, ITransferServiceFactory transferServiceFactory)
        {
            _context = context;
            _repository = repository;
            _transferServiceFactory = transferServiceFactory;
            _uploads = new List<IUpload>();
            _lock = new object();
        }

        public async Task<bool> BeginTesting(IUpload upload)
        {
            if (!AddToList(upload))
            {
                return false;
            }

            var server = _repository.GetFreeServer();
            if (server == null)
            {
                return false;
            }

            var transferService = _transferServiceFactory.Create(server.ServerAddress, 10000);

            var result = await CacheChecker(server, upload.Task.Checker, transferService);

            if (!result)
            {
                _repository.AddBrokenServer(server.ServerAddress);
                return result;
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
                _repository.AddBrokenServer(server.ServerAddress);
            }
            return result;
        }

        public async Task ReportResult(CheckerResult result)
        {
            if (result.Type.ToLower() == "code")
            {
                await ReportCodeResult(result);
            }
            else throw new NotImplementedException();
        }

        private async Task ReportCodeResult(CheckerResult result)
        {
            var upload = await _context.CodeUploads
                .SingleOrDefaultAsync(u => u.UploadId == result.UploadId);
            var userTask = await _context.UserTasks
                .SingleOrDefaultAsync(u => u.UserId == upload.UserId
                && u.TaskId == upload.TaskId);
            var totalTests = await _context.Tasks
                .Where(t => t.Id == upload.TaskId)
                .Select(t => t.Tests)
                .CountAsync();
            upload.Points = (double)result.SuccessfulTests / totalTests;
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
            var save = _context.SaveChangesAsync();
            lock (_lock)
            {
                var listUpload = _uploads.Find(u => u.UploadId == result.UploadId);
                _uploads.Remove(listUpload);
            }
            await save;
        }

        private bool AddToList(IUpload upload)
        {
            lock (_lock)
            {
                _uploads.Add(upload);
            }
            upload.Status = UploadStatus.Queue;
            _context.Add(upload);
            _context.SaveChanges();
            lock (_lock)
            {
                return _uploads.Count <= _repository.ServersCount;
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
            if (!server.CachedCheckerIds.Contains(checker.Id))
            {
                result = await SendChecker(checker, transferService);
                if (result)
                {
                    server.CachedCheckerIds.Add(checker.Id);
                }
            }
            return result;
        }

        public void ReportFreeServer(NewServer server)
        {
            _repository.AddFreeServer(server.Ip);
        }

        public void RemoveCachedChecker(Guid checkerId)
        {
            _repository.RemoveCachedCheckers(checkerId);
        }
    }
}
