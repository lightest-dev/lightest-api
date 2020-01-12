using System;
using System.Linq;
using System.Threading.Tasks;
using Lightest.AccessService.Interfaces;
using Lightest.Data;
using Lightest.Data.Models;
using Lightest.Data.Models.TaskModels;
using Microsoft.AspNetCore.Identity;

namespace Lightest.AccessService.RoleBasedAccessServices
{
    public class UploadsAccessService : RoleChecker, IAccessService<Upload>
    {
        private readonly IAccessService<TaskDefinition> _accessService;
        private readonly RelationalDbContext _context;

        public UploadsAccessService(
            RelationalDbContext context,
            UserManager<ApplicationUser> userManager,
            IAccessService<TaskDefinition> accessService) : base(userManager)
        {
            _accessService = accessService;
            _context = context;
        }

        public bool HasAdminAccess(ApplicationUser requester) => IsAdmin(requester).GetAwaiter().GetResult();

        public Task<bool> HasReadAccess(Guid id, ApplicationUser requester)
        {
            var taskId = _context.Uploads.Where(u => u.Id == id).Select(u => u.TaskId).First();
            return _accessService.HasReadAccess(taskId, requester);
        }

        public bool HasWriteAccess(Upload upload, ApplicationUser requester) => _accessService.HasReadAccess(upload.Task.Id, requester)
            .GetAwaiter().GetResult();
    }
}
