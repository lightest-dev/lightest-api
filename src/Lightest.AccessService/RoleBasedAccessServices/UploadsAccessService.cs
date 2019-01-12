using Lightest.AccessService.Interfaces;
using Lightest.Data.Models;
using Lightest.Data.Models.TaskModels;
using Microsoft.AspNetCore.Identity;

namespace Lightest.AccessService.RoleBasedAccessServices
{
    public class UploadsAccessService : RoleChecker, IAccessService<IUpload>
    {
        private readonly IAccessService<TaskDefinition> _accessService;

        public UploadsAccessService(
            UserManager<ApplicationUser> userManager,
            IAccessService<TaskDefinition> accessService) : base(userManager)
        {
            _accessService = accessService;
        }

        public bool CheckAdminAccess(IUpload upload, ApplicationUser requester)
        {
            return IsAdmin(requester);
        }

        public bool CheckReadAccess(IUpload upload, ApplicationUser requester)
        {
            return _accessService.CheckReadAccess(upload.Task, requester);
        }

        public bool CheckWriteAccess(IUpload upload, ApplicationUser requester)
        {
            return _accessService.CheckReadAccess(upload.Task, requester);
        }
    }
}
