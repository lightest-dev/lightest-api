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
        public Task AddResult([FromBody] CheckerResult result)
        {
            return _testingService.ReportResult(result);
        }

        [HttpPost("checker-result")]
        public async Task AddResult([FromBody] CheckerCompilationResult result)
        {
            var checker = await _context.Checkers.FindAsync(result.Id);
            checker.Compiled = result.Compiled;
            checker.Message = result.Message;
            await _context.SaveChangesAsync();
        }

        [HttpPost("free")]
        public Task ReportFreeServer([FromBody] NewServer server)
        {
            server.Ip = _accessor.HttpContext.Connection.RemoteIpAddress;
            _testingService.ReportFreeServer(server);
            return Task.CompletedTask;
        }

        [HttpPost("error")]
        public Task ReportError([FromBody] TestingError error)
        {
            error.Ip = _accessor.HttpContext.Connection.RemoteIpAddress;
            _logger.LogError("{Ip}:{ErrorMessage}", error.Ip, error.ErrorMessage);
            _testingService.ReportFreeServer(error);
            return Task.CompletedTask;
        }
    }
}
