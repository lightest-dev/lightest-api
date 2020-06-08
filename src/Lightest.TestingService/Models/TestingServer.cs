using Lightest.Data.Models;
using Lightest.TestingService.Interfaces;

namespace Lightest.TestingService.Models
{
    public class TestingServer
    {
        public TestingServer(TestingServerInfo serverInfo, ITransferService transferService)
        {
            ServerInfo = serverInfo;
            TransferService = transferService;
        }

        public TestingServerInfo ServerInfo { get; }

        public ITransferService TransferService { get; }

        public string DefaultKey => $"{ServerInfo.Ip}:{ServerInfo.Port.ToString()}";
    }
}
