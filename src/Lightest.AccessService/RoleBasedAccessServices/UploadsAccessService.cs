using Lightest.AccessService.Interfaces;
using Lightest.Data.Models;
using Lightest.Data.Models.TaskModels;

namespace Lightest.AccessService.RoleBasedAccessServices
{
    public class UploadsAccessService : IAccessService<IUpload>
    {
        public bool CheckAdminAccess(IUpload upload, ApplicationUser requester)
        {
            return true;
        }

        public bool CheckReadAccess(IUpload upload, ApplicationUser requester)
        {
            return true;
        }

        public bool CheckWriteAccess(IUpload upload, ApplicationUser requester)
        {
            return true;
        }
    }
}
