using System;
using System.Threading.Tasks;
using Grpc.Core;
using GrpcServices;
using Lightest.TestingService.DefaultServices;
using Lightest.TransferTest.Properties;
using Microsoft.Extensions.Logging;

namespace Lightest.TransferTest
{
    internal class Program
    {
        private static readonly Uri TestingServerUri = new Uri("https://localhost:443");
        private static readonly Guid CheckerId = Guid.Parse("A2D88393-24E7-4EE9-A491-35B9F8B707EF");
        private static readonly Guid UploadId = Guid.Parse("F9B7998F-66C5-447B-ADFA-E2E0F78C01BF");

        private static async Task Main(string[] args)
        {
            var loggerFactory = LoggerFactory.Create(builder =>
            {
                builder
                    .AddConsole();
            });

            // TODO: set up HTTPS credentials
            var transferService = new TransferService(loggerFactory, TestingServerUri, ChannelCredentials.Insecure);

            await SendChecker(transferService);
            await SendUpload(transferService);
        }

        private static async Task SendChecker(TransferService transferService)
        {
            var checkerRequest = new CheckerRequest
            {
                Code = Resources.IntSequenceChecker,
                Id = CheckerId.ToString()
            };

            var result = await transferService.SendChecker(checkerRequest);
        }

        private static async Task SendUpload(TransferService transferService)
        {
            var request = new TestingRequest
            {
                UploadId = UploadId.ToString(),
                CheckerId = CheckerId.ToString(),
                MemoryLimit = 512,
                TimeLimit = 500,
                CodeFile = new CodeFile
                {
                    Code = Resources.SampleUploadCode,
                    FileName = $"{UploadId}.cpp"
                },
            };
            request.Tests.Add(new Test
            {
                Input = "4\n3",
                Output = "7\n-1",
                TestName = "001"
            });
            request.Tests.Add(new Test
            {
                Input = "2\n6",
                Output = "8\n4",
                TestName = "002"
            });

            var result = await transferService.SendUpload(request);
        }

        private static async Task CheckStatus(TransferService transferService)
        {
            var result = await transferService.GetStatus();
        }
    }
}
