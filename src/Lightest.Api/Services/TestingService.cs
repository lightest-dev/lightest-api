using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Lightest.Api.ViewModels;
using Lightest.Data;
using Lightest.Data.Models.TaskModels;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace Lightest.Api.Services
{
    public class TestingService : ITestingService
    {
        private readonly RelationalDbContext _context;
        private readonly ServerRepostitory _repostitory;
        private readonly List<IUpload> _uploads;

        public TestingService(ServerRepostitory repostitory, RelationalDbContext context)
        {
            _context = context;
            _repostitory = repostitory;
            _uploads = new List<IUpload>();
        }

        public async Task<bool> BeginTesting(IUpload upload)
        {
            if (!AddToList(upload))
            {
                return false;
            }
            bool result;
            var transferService = _repostitory.GetFreeServer();
            if (transferService == null)
            {
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
                upload.Status = "Testing";
                await _context.SaveChangesAsync();
            }
            else
            {
                result = AddToList(upload);
            }
            return result;
        }

        public async Task ReportCodeResult(CheckerResult result)
        {
            var upload = await _context.CodeUploads.Include(u => u.Task).SingleOrDefaultAsync(u => u.UploadId == result.UploadId);
            var totalTests = upload.Task.Tests.Count;
            upload.Points = (double)result.SuccessfulTests / totalTests;
            upload.Status = result.Status;
            upload.Message = result.Message;
            var save = _context.SaveChangesAsync();
            var listUpload = _uploads.Find(u => u.UploadId == result.UploadId);
            _uploads.Remove(listUpload);
            await save;
        }

        public async Task ReportResult(CheckerResult result)
        {
            if (result.Type == "Code")
            {
                await ReportCodeResult(result);
            }
            else throw new NotImplementedException();
        }

        private bool AddToList(IUpload upload)
        {
            _uploads.Add(upload);
            upload.Status = "Queue";
            _context.Add(upload);
            _context.SaveChanges();
            return _uploads.Count == 1;
        }

        private async Task<bool> SendData(CodeUpload upload, ITransferService transferService)
        {
            upload.Status = "Queue";
            var save = _context.SaveChangesAsync();
            var language = upload.Task.Languages.FirstOrDefault(l => l.LanguageId == upload.LanguageId);
            var request = new TestingRequestViewModel
            {
                UploadId = upload.UploadId,
                MemoryLimit = language.MemoryLimit,
                TimeLimit = language.TimeLimit,
                Extension = upload.Language.Extension,
                TestsCount = upload.Task.Tests.Count,
                Type = "Code"
            };
            var result = await transferService.SendMessage(JsonConvert.SerializeObject(request));
            if (!result)
            {
                return false;
            }
            var i = 0;
            foreach (var test in upload.Task.Tests)
            {
                result = await transferService.SendFile($"{i}.in", Encoding.UTF8.GetBytes(test.Input));
                if (!result)
                {
                    return false;
                }
                result = await transferService.SendFile($"{i}.out", Encoding.UTF8.GetBytes(test.Output));
                if (!result)
                {
                    return false;
                }
                i++;
            }
            await save;
            result = await transferService.SendFile($"code.{upload.Language.Extension}", Encoding.UTF8.GetBytes(upload.Code));
            return result;
        }
    }
}
