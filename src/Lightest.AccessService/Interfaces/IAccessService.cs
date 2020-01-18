using System;
using System.Threading.Tasks;
using Lightest.Data.Models;

namespace Lightest.AccessService.Interfaces
{
    public interface IAccessService<in T>
    {
        bool HasAdminAccess(ApplicationUser requester);

        Task<bool> HasReadAccess(Guid id, ApplicationUser requester);

        Task<bool> HasWriteAccess(Guid id, ApplicationUser requester);
    }
}
