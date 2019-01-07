using System.Net;
using Lightest.TestingService.Interfaces;

namespace Lightest.TestingService.DefaultServices
{
    public class TransferServiceFactory : ITransferServiceFactory
    {
        public ITransferService Create(IPAddress ip, int port)
        {
            return new TransferService(ip, port);
        }
    }
}
