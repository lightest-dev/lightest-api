using System.Threading.Tasks;
using Lightest.TestingService.Models;

namespace Lightest.TestingService.Interfaces
{
    /// <summary>
    /// Use to transfer data to testing container. Listener in container should understand the format of data.
    /// </summary>
    public interface ITransferService
    {
        Task<bool> SendFile(string path, FileRequestType fileType);

        Task<bool> SendFile(string filename, FileRequestType fileType, byte[] data);

        Task<bool> SendMessage(string message);
    }
}
