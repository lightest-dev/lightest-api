using Lightest.Data.Models;
using Lightest.Data.Models.TaskModels;

namespace Lightest.Api.Services.AccessServices
{
    public class UploadsAccessService : IAccessService<IUpload>
    {
        public bool CheckAdminAccess(IUpload category, ApplicationUser user)
        {
            return true;
        }

        public bool CheckReadAccess(IUpload category, ApplicationUser user)
        {
            return true;
        }

        public bool CheckWriteAccess(IUpload category, ApplicationUser user)
        {
            return true;
        }
    }
}
