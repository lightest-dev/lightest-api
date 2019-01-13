using System.Threading.Tasks;
using Lightest.Api.RequestModels;
using Lightest.Data;
using Lightest.TestingService.Interfaces;
using Lightest.TestingService.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

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

        private IHttpContextAccessor _accessor;

        public TestingController(
            ITestingService testingService,
            RelationalDbContext context,
            IHttpContextAccessor accessor)
        {
            _testingService = testingService;
            _context = context;
            _accessor = accessor;
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
    }
}
