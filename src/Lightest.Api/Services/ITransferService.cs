using System.Threading.Tasks;

namespace Lightest.Api.Services
{
    /// <summary>
    /// Use to transfer data to testing container. Listener in container should understand the format of data.
    /// </summary>
    public interface ITransferService
    {
        Task<bool> SendMessage(string message);

        Task<bool> SendFile(string path);

        Task<bool> SendFile(string filename, byte[] data);
    }
}