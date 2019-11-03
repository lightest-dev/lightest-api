using Lightest.AccessService.Interfaces;
using Lightest.Data.Models;
using Lightest.Data.Models.TaskModels;

namespace Lightest.AccessService.MockAccessServices
{
    public class UploadsAccessService : IAccessService<Upload>
    {
        public bool HasAdminAccess(ApplicationUser requester) => true;

        public bool HasReadAccess(Upload upload, ApplicationUser requester) => true;

        public bool HasWriteAccess(Upload upload, ApplicationUser requester) => true;
    }
}
