using System;
using System.IO;
using System.Net;
using Grpc.Core;
using Lightest.TestingService.DefaultServices;
using Lightest.TestingService.Interfaces;
using Lightest.TestingService.Models;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Lightest.TestingService.Factories
{
    public class TransferServiceFactory : ITransferServiceFactory
    {
        private readonly ILoggerFactory _loggerFactory;
        private readonly IOptions<GrpcSettings> _settings;

        public TransferServiceFactory(ILoggerFactory loggerFactory, IOptions<GrpcSettings> settings)
        {
            _loggerFactory = loggerFactory;
            _settings = settings;
        }

        public ITransferService Create(IPAddress ip, int port)
        {
            var currentSettings = _settings.Value;
            var address = new UriBuilder("http", ip.ToString(), port);
            ChannelCredentials credentials;

            if (currentSettings.Insecure)
            {
                credentials = ChannelCredentials.Insecure;
            }
            else
            {
                address.Scheme = "https";
                credentials = new SslCredentials(File.ReadAllText(currentSettings.CertificatePath));
            }

            return new TransferService(_loggerFactory,
                address.Uri, credentials);
        }
    }
}
