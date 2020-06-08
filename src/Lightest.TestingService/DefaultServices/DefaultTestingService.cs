using System.Threading.Tasks;
using Lightest.Data;
using Lightest.Data.CodeManagment.Services;
using Lightest.Data.Models;
using Lightest.Data.Models.TaskModels;
using Lightest.TestingService.Interfaces;

namespace Lightest.TestingService.DefaultServices
{
    public class DefaultTestingService : ITestingService
    {
        private readonly RelationalDbContext _context;

        public DefaultTestingService(RelationalDbContext context)
        {
            _context = context;
        }

        public async Task AddToTestingQueue(Upload upload)
        {
            upload.Status = UploadStatus.Queue;
            await _context.SaveChangesAsync();
        }

        public Task ReportNewServer(string ip)
        {
            var server = new TestingServerInfo
            {
                Ip = ip,
                Port = 443,
                Status = ServerStatus.Free
            };
            // TODO
            //_repository.AddNewServer(server);
            return Task.CompletedTask;
        }
    }
}
