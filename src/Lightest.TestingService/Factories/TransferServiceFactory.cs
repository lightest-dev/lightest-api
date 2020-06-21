using System;
using System.Net;
using Grpc.Core;
using Lightest.TestingService.DefaultServices;
using Lightest.TestingService.Interfaces;
using Microsoft.Extensions.Logging;

namespace Lightest.TestingService.Factories
{
    public class TransferServiceFactory : ITransferServiceFactory
    {
        private readonly ILoggerFactory _loggerFactory;

        public TransferServiceFactory(ILoggerFactory loggerFactory)
        {
            _loggerFactory = loggerFactory;
        }

        public ITransferService Create(IPAddress ip, int port)
        {
            var uriBuilder = new UriBuilder("https", ip.ToString(), port);
            return new TransferService(_loggerFactory,
            // TODO: set up HTTPS credentials
                uriBuilder.Uri, ChannelCredentials.Insecure);
        }
    }
}
