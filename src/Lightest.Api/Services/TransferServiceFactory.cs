using System.Net;

namespace Lightest.Api.Services
{
    public class TransferServiceFactory : ITransferServiceFactory
    {
        public ITransferService Create(IPAddress ip, int port)
        {
            return new TransferService(ip, port);
        }
    }
}
