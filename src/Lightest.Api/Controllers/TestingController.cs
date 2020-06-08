using System.Threading.Tasks;
using Lightest.Data;
using Lightest.TestingService.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Lightest.Api.Controllers
{
    /// <summary>
    /// Use to report testing results to API.
    /// </summary>
    [Route("[controller]")]
    [ApiExplorerSettings(IgnoreApi = true)]
    [AllowAnonymous]
    public class TestingController : ControllerBase
    {
        private readonly ITestingService _testingService;

        private readonly RelationalDbContext _context;

        private readonly IHttpContextAccessor _accessor;

        private readonly ILogger _logger;

        public TestingController(
            ITestingService testingService,
            RelationalDbContext context,
            IHttpContextAccessor accessor,
            ILogger<TestingController> logger)
        {
            _testingService = testingService;
            _context = context;
            _accessor = accessor;
            _logger = logger;
        }

        [HttpPost("new")]
        public async Task<IActionResult> ReportNewServer()
        {
            var ip = _accessor.HttpContext.Connection.RemoteIpAddress.ToString();
            await _testingService.ReportNewServer(ip);
            return Ok();
        }
    }
}
