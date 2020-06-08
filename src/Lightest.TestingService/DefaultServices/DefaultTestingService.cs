using System.Threading.Tasks;
using Lightest.Data;
using Lightest.Data.Models;
using Lightest.Data.Models.TaskModels;
using Lightest.TestingService.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Lightest.TestingService.DefaultServices
{
    public class DefaultTestingService : ITestingService
    {
        private readonly RelationalDbContext _context;
        private readonly ITestingRunner _testingRunner;

        public DefaultTestingService(RelationalDbContext context, ITestingRunner testingRunner)
        {
            _context = context;
            _testingRunner = testingRunner;
        }

        public async Task AddToTestingQueue(Upload upload)
        {
            upload.Status = UploadStatus.Queue;
            await _context.SaveChangesAsync();

            _ = _testingRunner.TryStartNewWorker();
        }

        public async Task ReportNewServer(string ip)
        {
            var server = new TestingServerInfo
            {
                Ip = ip,
                Port = 443,
                Status = ServerStatus.Free
            };
            var existingServer = await _context.Servers.FirstOrDefaultAsync(s => s.Ip == ip);
            if (existingServer == null)
            {
                _context.Servers.Add(server);
            }
            else
            {
                existingServer.Port = server.Port;
                existingServer.Status = ServerStatus.Free;
            }
            await _context.SaveChangesAsync();

            _ = _testingRunner.TryStartNewWorker();
        }
    }
}
