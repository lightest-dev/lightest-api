using System.Linq;
using System.Threading.Tasks;
using Grpc.Core;
using Lightest.Data;
using Lightest.Data.CodeManagment.Services;
using Lightest.Data.Models;
using Lightest.Data.Models.TaskModels;
using Lightest.TestingService.Interfaces;
using Lightest.TestingService.Models;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Lightest.TestingService.DefaultServices
{
    public class TestingRunner : ITestingRunner
    {
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly ITransferServiceFactory _transferServiceFactory;
        private readonly IUploadProcessorFactory _uploadProcessorFactory;
        private readonly ILogger<TestingRunner> _logger;

        public TestingRunner(IServiceScopeFactory scopeFactory, ITransferServiceFactory transferServiceFactory,
            IUploadProcessorFactory uploadProcessorFactory, ILogger<TestingRunner> logger)
        {
            _scopeFactory = scopeFactory;
            _transferServiceFactory = transferServiceFactory;
            _uploadProcessorFactory = uploadProcessorFactory;
            _logger = logger;
        }

        public async Task TryStartNewWorker()
        {
            using var scope = _scopeFactory.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<RelationalDbContext>();
            var codeManagmentService = scope.ServiceProvider.GetRequiredService<ICodeManagmentService>();

            var serverInfo = context.Servers.FirstOrDefault(s => s.Status == Data.Models.ServerStatus.Free);
            if (serverInfo == null)
            {
                return;
            }

            using var transferService = _transferServiceFactory.Create(serverInfo.IPAddress, serverInfo.Port);
            var server = new TestingServer(serverInfo, transferService);

            var upload = context.Uploads.FirstOrDefault(c => c.Status == UploadStatus.Queue);

            while (upload != null)
            {
                var uploadData = codeManagmentService.Get(upload.Id);

                serverInfo.Status = ServerStatus.Busy;
                upload.Status = "In Progress";

                await context.SaveChangesAsync();

                var processor = _uploadProcessorFactory.Create(upload, uploadData,
                    server, context);
            }

            serverInfo.Status = ServerStatus.Free;
            await context.SaveChangesAsync();
        }

        public async Task UpdateServerStatuses()
        {
            using var scope = _scopeFactory.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<RelationalDbContext>();

            var servers = context.Servers.ToList();
            foreach (var serverInfo in servers)
            {
                try
                {
                    using var transferService = _transferServiceFactory.Create(serverInfo.IPAddress, serverInfo.Port);
                    var status = await transferService.GetStatus();
                    if (status.Free)
                    {
                        serverInfo.Status = ServerStatus.Free;
                    }
                    else
                    {
                        serverInfo.Status = ServerStatus.Busy;
                    }
                }
                catch (RpcException)
                {
                    serverInfo.Status = ServerStatus.NotResponding;
                    _logger.LogError("Server {Server}:{Port} is broken.", serverInfo.Ip, serverInfo.Port);
                }

                await context.SaveChangesAsync();
            }
        }
    }
}
