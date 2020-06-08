using Lightest.CodeManagment.Models;
using Lightest.Data;
using Lightest.Data.Models.TaskModels;
using Lightest.TestingService.Models;

namespace Lightest.TestingService.Interfaces
{
    public interface IUploadProcessorFactory
    {
        IUploadProcessor Create(Upload upload, UploadData uploadData,
            TestingServer testingServer, RelationalDbContext context);
    }
}
