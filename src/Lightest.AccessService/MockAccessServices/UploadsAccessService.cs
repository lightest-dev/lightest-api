using System;
using System.Threading.Tasks;
using Lightest.AccessService.Interfaces;
using Lightest.Data.Models;
using Lightest.Data.Models.TaskModels;

namespace Lightest.AccessService.MockAccessServices
{
    internal class UploadsAccessService : IAccessService<Upload>
    {
        public Task<bool> CanAdd(Upload item, ApplicationUser requester) => Task.FromResult(true);

        public Task<bool> CanRead(Guid id, ApplicationUser requester) => Task.FromResult(true);

        public Task<bool> CanWrite(Guid id, ApplicationUser requester) => Task.FromResult(true);
    }
}
