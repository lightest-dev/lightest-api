using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using Lightest.TestingService.Interfaces;
using Lightest.TestingService.Requests;

namespace Lightest.TestingService.DefaultServices
{
    public class TransferService : ITransferService
    {
        private readonly IPEndPoint _endpoint;

        public TransferService(IPAddress ip, int port)
        {
            _endpoint = new IPEndPoint(ip, port);
        }

        public async Task<bool> SendFile(FileRequest fileRequest, string path)
        {
            if (!File.Exists(path))
            {
                return false;
            }
            using (var client = new TcpClient())
            {
                await client.ConnectAsync(_endpoint.Address, _endpoint.Port);
                if (!client.Connected)
                {
                    return false;
                }
                using (var netStream = client.GetStream())
                using (var writer = new BinaryWriter(netStream))
                using (var fileStream = File.OpenRead(path))
                {
                    var message = fileRequest.ToString();
                    var bytes = Encoding.UTF8.GetBytes(message);
                    // 1 for type, 4 for message length
                    var length = fileStream.Length + bytes.Length + 1 + 4;
                    writer.Write(length);
                    writer.Write(RequestType.File);
                    writer.Write(bytes.Length);
                    writer.Write(bytes);
                    await fileStream.CopyToAsync(netStream);
                }
            }
            return true;
        }

        public async Task<bool> SendFile(FileRequest fileRequest, byte[] data)
        {
            using (var client = new TcpClient())
            {
                await client.ConnectAsync(_endpoint.Address, _endpoint.Port);
                if (!client.Connected)
                {
                    return false;
                }
                using (var netStream = client.GetStream())
                using (var writer = new BinaryWriter(netStream))
                {
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
            }
            return true;
        }

        public async Task<bool> SendMessage(string message)
        {
            using (var client = new TcpClient())
            {
                await client.ConnectAsync(_endpoint.Address, _endpoint.Port);
                if (!client.Connected)
                {
                    return false;
                }
                using (var stream = client.GetStream())
                using (var writer = new BinaryWriter(stream))
                {
                    var bytes = Encoding.UTF8.GetBytes(message);
                    long length = bytes.Length + 1;
                    writer.Write(length);
                    writer.Write(RequestType.Message);
                    writer.Write(bytes);
                }
            }
            return true;
        }
    }
}
