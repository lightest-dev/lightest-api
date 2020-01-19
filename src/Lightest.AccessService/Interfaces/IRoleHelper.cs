using System.Threading.Tasks;
using Lightest.Data.Models;

namespace Lightest.AccessService.Interfaces
{
    public interface IRoleHelper
    {
        Task<bool> IsAdmin(ApplicationUser user);

        Task<bool> IsTeacher(ApplicationUser user);
    }
}
