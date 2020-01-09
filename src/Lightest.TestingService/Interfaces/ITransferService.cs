using System.Threading.Tasks;
using Lightest.TestingService.RequestModels;

namespace Lightest.TestingService.Interfaces
{
    /// <summary>
    /// Use to transfer data to testing container. Listener in container should understand the format of data.
    /// </summary>
    public interface ITransferService
    {
        Task<bool> SendFile(FileRequest fileRequest, string path);

        Task<bool> SendFile(FileRequest fileRequest, byte[] data);

        Task<bool> SendMessage(string message);
    }
}
