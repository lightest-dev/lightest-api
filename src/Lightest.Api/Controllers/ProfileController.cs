using Lightest.Data;
using Microsoft.AspNetCore.Mvc;

namespace Lightest.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProfileController : Controller
    {
        private readonly RelationalDbContext _context;

        public ProfileController(RelationalDbContext context)
        {
            _context = context;
        }
    }
}