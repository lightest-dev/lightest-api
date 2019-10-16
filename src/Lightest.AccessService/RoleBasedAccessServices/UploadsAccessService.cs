using Lightest.AccessService.Interfaces;
using Lightest.Data.Models;
using Lightest.Data.Models.TaskModels;
using Microsoft.AspNetCore.Identity;

namespace Lightest.AccessService.RoleBasedAccessServices
{
    public class UploadsAccessService : RoleChecker, IAccessService<Upload>
    {
        private readonly IAccessService<TaskDefinition> _accessService;

        public UploadsAccessService(
            UserManager<ApplicationUser> userManager,
            IAccessService<TaskDefinition> accessService) : base(userManager) => _accessService = accessService;

        public bool CheckAdminAccess(Upload upload, ApplicationUser requester) => IsAdmin(requester);

        public bool CheckReadAccess(Upload upload, ApplicationUser requester) => _accessService.CheckReadAccess(upload.Task, requester);

        public bool CheckWriteAccess(Upload upload, ApplicationUser requester) => _accessService.CheckReadAccess(upload.Task, requester);
    }
}
