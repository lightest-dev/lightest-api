using System.Net;

namespace Lightest.TestingService.Interfaces
{
    public interface ITransferServiceFactory
    {
        ITransferService Create(IPAddress ip, int port);
    }
}
