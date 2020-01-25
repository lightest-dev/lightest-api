using System;
using System.Linq;
using System.Threading.Tasks;
using Lightest.AccessService.Interfaces;
using Lightest.Data;
using Lightest.Data.Models;
using Lightest.Data.Models.TaskModels;

namespace Lightest.AccessService.RoleBasedAccessServices
{
    internal class UploadsAccessService : BaseAccessService, IAccessService<Upload>
    {
        private readonly IAccessService<TaskDefinition> _accessService;
        private readonly RelationalDbContext _context;

        public UploadsAccessService(
            RelationalDbContext context,
            IRoleHelper roleHelper,
            IAccessService<TaskDefinition> accessService) : base(roleHelper)
        {
            _accessService = accessService;
            _context = context;
        }

        public Task<bool> HasReadAccess(Guid id, ApplicationUser requester)
        {
            var taskId = _context.Uploads.Where(u => u.Id == id).Select(u => u.TaskId).First();
            return _accessService.HasReadAccess(taskId, requester);
        }

        public Task<bool> HasWriteAccess(Guid id, ApplicationUser requester)
        {
            var taskId = _context.Uploads.Where(u => u.Id == id).Select(u => u.TaskId).First();
            return _accessService.HasReadAccess(taskId, requester);
        }
    }
}
