using System.Threading.Tasks;
using Lightest.Api.RequestModels;
using Lightest.Data;
using Lightest.TestingService.Interfaces;
using Lightest.TestingService.Models;
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

        [HttpPost("result")]
        public async Task<IActionResult> AddResult([FromBody] CheckerResult result)
        {
            if (result.Ip == null)
            {
                result.Ip = _accessor.HttpContext.Connection.RemoteIpAddress.ToString();
            }
            await Task.WhenAll(_testingService.ReportResult(result), 
                _testingService.StartNextTesting());
            return Ok();
        }

        [HttpPost("checker-result")]
        public async Task<IActionResult> AddCheckerResult([FromBody] CheckerCompilationResult result)
        {
            var checker = await _context.Checkers.FindAsync(result.Id);

            if (checker == null)
            {
                return BadRequest(nameof(result.Id));
            }

            checker.Compiled = result.Compiled;
            checker.Message = result.Message;
            await _context.SaveChangesAsync();
            return Ok();
        }

        [HttpPost("new")]
        public async Task<IActionResult> ReportNewServer([FromBody] NewServer server)
        {
            if (server.Ip == null)
            {
                server.Ip = _accessor.HttpContext.Connection.RemoteIpAddress.ToString();
            }
            await _testingService.ReportNewServer(server);
            return Ok();
        }

        [HttpPost("error")]
        public async Task<IActionResult> ReportError([FromBody] TestingError error)
        {
            if (error.Ip == null)
            {
                error.Ip = _accessor.HttpContext.Connection.RemoteIpAddress.ToString();
            }
            _logger.LogError("{Ip}:{ErrorMessage}", error.Ip, error.ErrorMessage);
            await _testingService.ReportBrokenServer(error);
            return Ok();
        }
    }
}
