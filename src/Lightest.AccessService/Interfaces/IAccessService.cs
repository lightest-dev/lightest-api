using System;
using System.Threading.Tasks;
using Lightest.Data.Models;

namespace Lightest.AccessService.Interfaces
{
    public interface IAccessService<in T>
    {
        Task<bool> CanRead(Guid id, ApplicationUser requester);

        Task<bool> CanWrite(Guid id, ApplicationUser requester);

        Task<bool> CanAdd(T item, ApplicationUser requester);
    }
}
