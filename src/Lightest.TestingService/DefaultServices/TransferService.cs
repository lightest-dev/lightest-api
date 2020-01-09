using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using Lightest.TestingService.Interfaces;
using Lightest.TestingService.RequestModels;
using Microsoft.Extensions.Logging;

namespace Lightest.TestingService.DefaultServices
{
    public class TransferService : ITransferService
    {
        //todo: refactor to use custom TcpClient for testing
        private readonly IPEndPoint _endpoint;

        private readonly ILogger _logger;

        public TransferService(ILogger logger, IPAddress ip, int port)
        {
            _logger = logger;
            _endpoint = new IPEndPoint(ip, port);
        }

        public async Task<bool> SendFile([NotNull]FileRequest fileRequest, string path)
        {
            if (!File.Exists(path))
            {
                return false;
            }

            try
            {
                using var client = new TcpClient();
                _logger.LogInformation($"Endpoint: {_endpoint.Address}:{_endpoint.Port.ToString()}");
                try
                {
                    await client.ConnectAsync(_endpoint.Address, _endpoint.Port);
                }
                catch (SocketException)
                {
                    await Task.Delay(3000);
                    await client.ConnectAsync(_endpoint.Address, _endpoint.Port);
                }
                if (!client.Connected)
                {
                    return false;
                }

                using var netStream = client.GetStream();
                using var writer = new BinaryWriter(netStream);
                using var fileStream = File.OpenRead(path);
                var message = fileRequest.ToString();
                var bytes = Encoding.UTF8.GetBytes(message);
                // 1 for type, 4 for message length
                var length = fileStream.Length + bytes.Length + 1 + 4;
                writer.Write(length);
                writer.Write(RequestType.File);
                writer.Write(bytes.Length);
                writer.Write(bytes);
                await fileStream.CopyToAsync(netStream);

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
                return false;
            }
        }

        public async Task<bool> SendFile(FileRequest fileRequest, byte[] data)
        {
            try
            {
                using (var client = new TcpClient())
                {
                    _logger.LogInformation($"Endpoint: {_endpoint.Address}:{_endpoint.Port.ToString()}");
                    try
                    {
                        await client.ConnectAsync(_endpoint.Address, _endpoint.Port);
                    }
                    catch (SocketException)
                    {
                        await Task.Delay(3000);
                        await client.ConnectAsync(_endpoint.Address, _endpoint.Port);
                    }
                    if (!client.Connected)
                    {
                        return false;
                    }
                    using var netStream = client.GetStream();
                    using var writer = new BinaryWriter(netStream);
                    var message = fileRequest.ToString();
                    var bytes = Encoding.UTF8.GetBytes(message);
                    // 1 for type, 4 for message length
                    long length = data.Length + bytes.Length + 1 + 4;
                    writer.Write(length);
                    writer.Write(RequestType.File);
                    writer.Write(bytes.Length);
                    writer.Write(bytes);
                    writer.Write(data);
                }
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
                return false;
            }
        }

        public async Task<bool> SendMessage(string message)
        {
            try
            {
                using (var client = new TcpClient())
                {
                    _logger.LogInformation($"Endpoint: {_endpoint.Address}:{_endpoint.Port.ToString()}");
                    try
                    {
                        await client.ConnectAsync(_endpoint.Address, _endpoint.Port);
                    }
                    catch (SocketException)
                    {
                        await Task.Delay(3000);
                        await client.ConnectAsync(_endpoint.Address, _endpoint.Port);
                    }
                    if (!client.Connected)
                    {
                        return false;
                    }
                    using var stream = client.GetStream();
                    using var writer = new BinaryWriter(stream);
                    var bytes = Encoding.UTF8.GetBytes(message);
                    long length = bytes.Length + 1;
                    writer.Write(length);
                    writer.Write(RequestType.Message);
                    writer.Write(bytes);
                }
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
                return false;
            }
        }
    }
}
