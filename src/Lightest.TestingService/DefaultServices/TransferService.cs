using System;
using System.Threading.Tasks;
using Grpc.Core;
using Grpc.Net.Client;
using GrpcServices;
using Lightest.TestingService.Interfaces;
using Microsoft.Extensions.Logging;

namespace Lightest.TestingService.DefaultServices
{
    public class TransferService : ITransferService
    {
        private bool _disposedValue;
        private readonly GrpcChannel _grpcChannel;
        private readonly CodeTester.CodeTesterClient _client;

        public TransferService(ILoggerFactory loggerFactory, Uri address, ChannelCredentials credentials)
        {
            _grpcChannel = GrpcChannel.ForAddress(address, new GrpcChannelOptions
            {
                Credentials = credentials,
                LoggerFactory = loggerFactory
            });

            _client = new CodeTester.CodeTesterClient(_grpcChannel);
        }

        public async Task<CheckerResponse> SendChecker(CheckerRequest request)
            => await _client.CompileCheckerAsync(request);

        public async Task<TestingResponse> SendUpload(TestingRequest request)
            => await _client.TestUploadAsync(request);

        public async Task<CheckerStatusResponse> GetStatus(CheckerStatusRequest request)
            => await _client.GetStatusAsync(request);

        public Task<CheckerStatusResponse> GetStatus()
            => GetStatus(new CheckerStatusRequest());

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposedValue)
            {
                if (disposing)
                {
                    _grpcChannel.Dispose();
                }

                _disposedValue = true;
            }
        }

        public void Dispose()
        {
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}
