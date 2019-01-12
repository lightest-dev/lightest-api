using System.Linq;
using System.Threading.Tasks;
using Lightest.Data;
using Lightest.Data.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Lightest.Api.Controllers
{
    public class BaseUserController : ControllerBase
    {
        protected readonly RelationalDbContext _context;
        protected readonly UserManager<ApplicationUser> _userManager;

        protected BaseUserController(RelationalDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        protected async Task<ApplicationUser> GetCurrentUser()
        {
            var id = User.Claims.SingleOrDefault(c => c.Type == "sub");
            var user = await _userManager.FindByIdAsync(id.Value);
            return user;
        }
    }
}
