using System;
using System.Threading.Tasks;
using Lightest.AccessService.Interfaces;
using Lightest.Data.Models;
using Lightest.Data.Models.TaskModels;

namespace Lightest.AccessService.MockAccessServices
{
    public class UploadsAccessService : IAccessService<Upload>
    {
        public bool HasAdminAccess(ApplicationUser requester) => true;

        public Task<bool> HasReadAccess(Guid id, ApplicationUser requester) => Task.FromResult(true);

        public bool HasWriteAccess(Upload upload, ApplicationUser requester) => true;
    }
}
