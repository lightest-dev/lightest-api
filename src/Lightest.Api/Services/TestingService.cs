using Lightest.Data;
using Lightest.Data.Models.TaskModels;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Lightest.Api.Services
{
    public class TestingService : ITestingService
    {
        private readonly List<IUpload> _uploads;
        private readonly RelationalDbContext _context;
        private readonly ServerRepostitory _repostitory;

        public TestingService(ServerRepostitory repostitory, RelationalDbContext context)
        {
            _context = context;
            _repostitory = repostitory;
            _uploads = new List<IUpload>();
        }

        private bool AddToList(IUpload upload)
        {
            _uploads.Add(upload);
            upload.Status = "Queue";
            _context.Add(upload);
            _context.SaveChanges();
            return _uploads.Count == 1;
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

        private async Task<bool> SendData(CodeUpload upload, ITransferService transferService)
        {
            upload.Status = "Queue";
            var save = _context.SaveChangesAsync();
            var result = await transferService.SendMessage($"code_upload:{upload.UploadId}");
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
            }
            await save;
            result = await transferService.SendFile($"code.{upload.Language.Extension}", Encoding.UTF8.GetBytes(upload.Code));
            return result;
        }

        public async Task ReportResult(CheckerResult result)
        {
            if (result.Type == "Code")
            {
                await ReportCodeResult(result);
            }
            else throw new NotImplementedException();
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
    }
}