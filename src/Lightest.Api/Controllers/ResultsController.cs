using System.Threading.Tasks;
using Lightest.Api.Services;
using Lightest.Data.Models.TaskModels;
using Microsoft.AspNetCore.Mvc;

namespace Lightest.Api.Controllers
{
    /// <summary>
    /// Use to report testing results to API.
    /// </summary>
    [Route("api/[controller]")]
    public class ResultsController : ControllerBase
    {
        private readonly ITestingService _testingService;

        public ResultsController(ITestingService testingService)
        {
            _testingService = testingService;
        }

        [HttpPost]
        public async Task AddResult([FromBody] CheckerResult result)
        {
            await _testingService.ReportResult(result);
        }
    }
}
