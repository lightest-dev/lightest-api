using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Lightest.Data;
using Lightest.Data.Models.TaskModels;

namespace Lightest.Api.Services
{
    public class TestingService : ITestingService
    {
        private readonly List<IUpload> _uploads;
        private readonly ITransferService _transferService;
        private readonly RelationalDbContext _context;

        public TestingService(ITransferService transferService, RelationalDbContext context)
        {
            _context = context;
            _transferService = transferService;
            _uploads = new List<IUpload>();
        }

        private bool AddToList(IUpload upload)
        {
            _uploads.Add(upload);
            return _uploads.Count == 1;
        }

        public async Task<bool> BeginTesting(IUpload upload)
        {
            if (!AddToList(upload))
            {
                return false;
            }
            switch (upload)
            {
                case CodeUpload code:
                    return await SendData(code);
                default:
                    throw new NotImplementedException();
            }
            
        }

        private async Task<bool> SendData(CodeUpload upload)
        {
            var result = await _transferService.SendMessage($"code_upload:{upload.UploadId}");
            if (!result)
            {
                return false;
            }
            var i = 0;
            foreach (var test in upload.Task.Tests)
            {
                result = await _transferService.SendFile($"{i}.in", Encoding.UTF8.GetBytes(test.Input));
                if (!result)
                {
                    return false;
                }
                result = await _transferService.SendFile($"{i}.out", Encoding.UTF8.GetBytes(test.Output));
                if (!result)
                {
                    return false;
                }
            }
            return await _transferService.SendFile($"code{upload.Language.Extension}", Encoding.UTF8.GetBytes(upload.Code));
        }

        public async Task<bool> CheckStatus(IUpload task)
        {
            throw new NotImplementedException();
        }

        public async Task<double> GetResult(IUpload task)
        {
            throw new NotImplementedException();
        }

        public Task ReportResult(CheckerResult result)
        {
            throw new NotImplementedException();
        }
    }
}
