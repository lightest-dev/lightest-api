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

        public Task<bool> CanAdd(Upload item, ApplicationUser requester)
        {
            var taskId = item.TaskId;
            return _accessService.CanRead(taskId, requester);
        }

        public async Task<bool> CanRead(Guid id, ApplicationUser requester)
        {
            var userId = _context.Uploads.Where(u => u.Id == id).Select(u => u.UserId).First();
            return userId == requester.Id || await IsTeacher(requester);
        }

        public async Task<bool> CanWrite(Guid id, ApplicationUser requester)
        {
            var userId = _context.Uploads.Where(u => u.Id == id).Select(u => u.UserId).First();
            return userId == requester.Id || await IsTeacher(requester);
        }
    }
}
