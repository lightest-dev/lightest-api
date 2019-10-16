using System.Net;
using Lightest.TestingService.Interfaces;
using Microsoft.Extensions.Logging;

namespace Lightest.TestingService.DefaultServices
{
    public class TransferServiceFactory : ITransferServiceFactory
    {
        private readonly ILoggerFactory _loggerFactory;

        public TransferServiceFactory(ILoggerFactory loggerFactory) => _loggerFactory = loggerFactory;

        public ITransferService Create(IPAddress ip, int port) => new TransferService(_loggerFactory.CreateLogger(typeof(TransferService)), ip, port);
    }
}
