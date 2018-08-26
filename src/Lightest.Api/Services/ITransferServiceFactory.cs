using System.Net;

namespace Lightest.Api.Services
{
    public interface ITransferServiceFactory
    {
        ITransferService Create(IPAddress ip, int port);
    }
}
