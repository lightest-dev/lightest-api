using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Grpc.Core;
using Lightest.Data.Models;
using Lightest.TestingService.Interfaces;
using Moq;
using Xunit;

namespace Lightest.Tests.TestingService.TestingRunner
{
    public class UpdateServerStatuses : BaseTest
    {
        private readonly Dictionary<TestingServerInfo, ServerStatus> _servers;

        public UpdateServerStatuses()
        {
            _transferServiceFactory.Setup(f => f.Create(It.IsAny<IPAddress>(), It.IsAny<int>()))
                .Returns(default(ITransferService));
            _servers = new Dictionary<TestingServerInfo, ServerStatus>();
        }

        private async Task SetUpServers()
        {
            foreach(var server in _servers)
            {
                _context.Servers.Add(server.Key);

                var transferService = new Mock<ITransferService>();

                switch (server.Value)
                {
                    case ServerStatus.Free:
                        transferService.Setup(s => s.GetStatus())
                            .ReturnsAsync(new GrpcServices.CheckerStatusResponse
                            {
                                Free = true
                            });
                        break;
                    case ServerStatus.Busy:
                        transferService.Setup(s => s.GetStatus())
                            .ReturnsAsync(new GrpcServices.CheckerStatusResponse
                            {
                                Free = false
                            });
                        break;
                    case ServerStatus.NotResponding:
                        transferService.Setup(s => s.GetStatus())
                            .Throws(new RpcException(Status.DefaultCancelled));
                        break;
                }

                _transferServiceFactory.Setup(f => f.Create(server.Key.IPAddress, server.Key.Port))
                    .Returns(transferService.Object);
            }

            await _context.SaveChangesAsync();
        }

        [Fact]
        public async Task NoServersExist()
        {
            // shouls still work correctly, even if no servers are present
            await _testingRunner.UpdateServerStatuses();
        }

        [Fact]
        public async Task AllServersUpdated()
        {
            _servers.Add(new TestingServerInfo
            {
                Ip = "1.1.1.1",
                Status = ServerStatus.Busy
            }, ServerStatus.Free);
            _servers.Add(new TestingServerInfo
            {
                Ip = "1.1.1.2",
                Status = ServerStatus.Busy
            }, ServerStatus.Busy);
            _servers.Add(new TestingServerInfo
            {
                Ip = "1.1.1.3",
                Status = ServerStatus.Busy
            }, ServerStatus.NotResponding);

            _servers.Add(new TestingServerInfo
            {
                Ip = "1.1.2.1",
                Status = ServerStatus.Free
            }, ServerStatus.Free);
            _servers.Add(new TestingServerInfo
            {
                Ip = "1.1.2.2",
                Status = ServerStatus.Free
            }, ServerStatus.Busy);
            _servers.Add(new TestingServerInfo
            {
                Ip = "1.1.2.3",
                Status = ServerStatus.Free
            }, ServerStatus.NotResponding);

            _servers.Add(new TestingServerInfo
            {
                Ip = "1.1.3.1",
                Status = ServerStatus.NotResponding
            }, ServerStatus.Free);
            _servers.Add(new TestingServerInfo
            {
                Ip = "1.1.3.2",
                Status = ServerStatus.NotResponding
            }, ServerStatus.Busy);
            _servers.Add(new TestingServerInfo
            {
                Ip = "1.1.3.3",
                Status = ServerStatus.NotResponding
            }, ServerStatus.NotResponding);
            await SetUpServers();

            await _testingRunner.UpdateServerStatuses();

            foreach(var pair in _servers)
            {
                var server = _context.Servers.First(s => s.Ip == pair.Key.Ip);

                Assert.Equal(pair.Value, server.Status);
            }
        }
    }
}
