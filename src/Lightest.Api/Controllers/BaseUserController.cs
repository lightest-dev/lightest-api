using System.Linq;
using System.Threading.Tasks;
using Lightest.Data;
using Lightest.Data.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Lightest.Api.Controllers
{
    public class BaseUserController : ControllerBase
    {
        protected readonly RelationalDbContext _context;

        protected BaseUserController(RelationalDbContext context)
        {
            _context = context;
        }

        protected async Task<ApplicationUser> GetCurrentUser()
        {
            var id = User.Claims.SingleOrDefault(c => c.Type == "sub");
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == id.Value);
            return user;
        }
    }
}
