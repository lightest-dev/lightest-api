using System.Threading.Tasks;
using Lightest.Data.Models.TaskModels;
using Lightest.CodeManagment.Models;

namespace Lightest.TestingService.Interfaces
{
    public interface ITestingService
    {
        Task AddToTestingQueue(Upload upload);

        Task ReportNewServer(string ip);
    }
}
