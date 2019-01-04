using System.Threading.Tasks;

namespace Lightest.TestingService.Interfaces
{
    /// <summary>
    /// Use to transfer data to testing container. Listener in container should understand the format of data.
    /// </summary>
    public interface ITransferService
    {
        Task<bool> SendFile(string path);

        Task<bool> SendFile(string filename, byte[] data);

        Task<bool> SendMessage(string message);
    }
}
