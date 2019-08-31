using Lightest.AccessService.Interfaces;
using Lightest.Data.Models;
using Lightest.Data.Models.TaskModels;

namespace Lightest.AccessService.MockAccessServices
{
    public class UploadsAccessService : IAccessService<Upload>
    {
        public bool CheckAdminAccess(Upload upload, ApplicationUser requester)
        {
            return true;
        }

        public bool CheckReadAccess(Upload upload, ApplicationUser requester)
        {
            return true;
        }

        public bool CheckWriteAccess(Upload upload, ApplicationUser requester)
        {
            return true;
        }
    }
}
