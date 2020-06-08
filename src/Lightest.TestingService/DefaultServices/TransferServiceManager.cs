using System;
using System.Net;
using Grpc.Core;
using Lightest.TestingService.Interfaces;
using Microsoft.Extensions.Logging;

namespace Lightest.TestingService.DefaultServices
{
    public class TransferServiceManager : ITransferServiceFactory
    {
        private readonly ILoggerFactory _loggerFactory;

        public TransferServiceManager(ILoggerFactory loggerFactory)
        {
            _loggerFactory = loggerFactory;
        }

        public ITransferService Create(IPAddress ip, int port)
        {
            var uriBuilder = new UriBuilder("https", ip.ToString(), port);
            return new TransferService(_loggerFactory,
                uriBuilder.Uri, ChannelCredentials.Insecure);
        }
    }
}
