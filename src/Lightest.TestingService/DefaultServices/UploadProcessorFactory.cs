using Lightest.CodeManagment.Models;
using Lightest.Data;
using Lightest.Data.Models.TaskModels;
using Lightest.TestingService.Interfaces;
using Lightest.TestingService.Models;

namespace Lightest.TestingService.DefaultServices
{
    public class UploadProcessorFactory : IUploadProcessorFactory
    {
        public IUploadProcessor Create(Upload upload, UploadData uploadData,
            TestingServer testingServer, RelationalDbContext context)
            => new UploadProcessor(upload, uploadData, testingServer, context);
    }
}
