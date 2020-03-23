using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Lightest.Data;
using Lightest.Data.Models;
using Lightest.Data.Models.TaskModels;
using Lightest.Data.Mongo.Services;
using Lightest.TestingService.Interfaces;
using Lightest.TestingService.RequestModels;
using Lightest.TestingService.ResponsModels;
using Microsoft.EntityFrameworkCore;

namespace Lightest.TestingService.DefaultServices
{
    public class DefaultTestingService : ITestingService
    {
        private readonly RelationalDbContext _context;
        private readonly IServerRepository _repository;
        private readonly ITransferServiceFactory _transferServiceFactory;
        private readonly IUploadDataRepository _uploadDataRepository;

        public DefaultTestingService(IServerRepository repository, RelationalDbContext context, ITransferServiceFactory transferServiceFactory, IUploadDataRepository uploadDataRepository)
        {
            _context = context;
            _repository = repository;
            _transferServiceFactory = transferServiceFactory;
            _uploadDataRepository = uploadDataRepository;
        }

        public async Task<bool> BeginTesting(Upload upload)
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

            result = upload switch
            {
                Upload code => await SendData(code, transferService),

                _ => throw new NotImplementedException(),
            };
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

        public Task ReportResult(CheckingResponse result)
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

            throw new NotSupportedException($"Uploads with type {result.Type} are currently not supported");
        }

        public Task StartNextTesting()
        {
            var upload = _context.Uploads.FirstOrDefault(c => c.Status == UploadStatus.Queue);

            //todo: refactor to enable complete testing, maybe move BeginTesting to separate class
            if (upload != null)
            {
                return BeginTesting(upload);
            }

            return Task.CompletedTask;
        }

        private async Task ReportCodeResult(CheckingResponse result)
        {
            var upload = await _context.Uploads
                .SingleOrDefaultAsync(u => u.Id == result.UploadId);
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

        private async Task AddToList(Upload upload)
        {
            upload.Status = UploadStatus.Queue;
            await StartTrackingCodeUpload(upload);
        }

        private async Task<bool> SendData(Upload upload, ITransferService transferService)
        {
            upload.Status = UploadStatus.Queue;
            var save = _context.SaveChangesAsync();
            var language = upload.Task.Languages.FirstOrDefault(l => l.LanguageId == upload.LanguageId);
            var request = new TestingRequest
            {
                UploadId = upload.Id,
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
                fileRequest = new TestRequest($"{i.ToString()}.in");
                result = await transferService.SendFile(fileRequest, Encoding.UTF8.GetBytes(test.Input));
                if (!result)
                {
                    return false;
                }
                fileRequest = new TestRequest($"{i.ToString()}.out");
                result = await transferService.SendFile(fileRequest, Encoding.UTF8.GetBytes(test.Output));
                if (!result)
                {
                    return false;
                }
                i++;
            }
            await save;
            fileRequest = new SingleFileCodeRequest($"{upload.Id.ToString()}.{upload.Language.Extension}");
            result = await transferService.SendFile(fileRequest, Encoding.UTF8.GetBytes(_uploadDataRepository.Get(upload.Id).Code));
            return result;
        }

        private static async Task<bool> SendChecker(Checker checker, ITransferService transferService)
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
        private async Task StartTrackingCodeUpload(Upload upload)
        {
            if (!_context.Uploads.Any(u => u.Id == upload.Id))
            {
                _context.Uploads.Add(upload);
                await _context.SaveChangesAsync();
            }
        }

        public Task ReportNewServer(ServerStatusResponse serverData)
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

        public Task ReportBrokenServer(ServerStatusResponse serverData)
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
