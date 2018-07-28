using Lightest.Api.Services;
using Lightest.Data;
using Lightest.Data.Models;
using Lightest.Data.Models.TaskModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

namespace Lightest.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UploadsController : Controller
    {
        private readonly ITestingService _testingService;
        private readonly RelationalDbContext _context;

        public UploadsController(ITestingService testingService, RelationalDbContext context)
        {
            _testingService = testingService;
            _context = context;
        }

        [HttpPost("code")]
        [ProducesResponseType(200, Type = typeof(int))]
        [ProducesResponseType(400)]
        [ProducesResponseType(403)]
        public async Task<IActionResult> UploadCode([FromBody] CodeUpload upload)
        {
            var user = GetCurrentUser();

            var task = await _context.Tasks
                            .Include(t => t.Languages)
                            .SingleOrDefaultAsync(t => t.Id == upload.TaskId);

            if (task == null)
            {
                return BadRequest();
            }
            if (!task.CheckReadAccess(user))
            {
                return Forbid();
            }

            var language = await _context.Languages.SingleOrDefaultAsync(l => l.Id == upload.LanguageId);
            if (language == null || !task.Languages.Any(l => l.LanguageId == upload.LanguageId))
            {
                return BadRequest();
            }

            upload.Status = "New";
            upload.Points = 0;
            upload.UserId = user.Id;

            _context.CodeUploads.Add(upload);
            await _context.SaveChangesAsync();

            var succesful = await _testingService.BeginTesting(upload);
            if (succesful)
            {
                return Ok(upload.UploadId);
            }
            return BadRequest();
        }

        [HttpPost("project")]
        [ProducesResponseType(200, Type = typeof(int))]
        [ProducesResponseType(400)]
        [ProducesResponseType(403)]
        public async Task<IActionResult> UploadProject([FromBody] ArchiveUpload upload)
        {
            var user = GetCurrentUser();

            var task = await _context.Tasks
                .Include(t => t.Languages)
                .SingleOrDefaultAsync(t => t.Id == upload.TaskId);

            if (task == null)
            {
                return BadRequest("task");
            }

            if (!task.CheckReadAccess(user))
            {
                return Forbid();
            }

            var language = await _context.Languages.SingleOrDefaultAsync(l => l.Id == upload.LanguageId);
            if (language == null || !task.Languages.Any(l => l.LanguageId == upload.LanguageId))
            {
                return BadRequest("language");
            }

            upload.Status = "New";
            upload.Points = 0;
            upload.UserId = user.Id;
            _context.ArchiveUploads.Add(upload);

            await _context.SaveChangesAsync();

            var succesful = await _testingService.BeginTesting(upload);
            if (succesful)
            {
                return Ok(upload.UploadId);
            }
            return BadRequest();
        }

        [HttpGet("{type}/{id}/status")]
        [ProducesResponseType(200, Type = typeof(bool))]
        [ProducesResponseType(400)]
        [ProducesResponseType(403)]
        public async Task<IActionResult> CheckStatus([FromRoute] string type, [FromRoute] int id)
        {
            IUpload upload;

            if (type.ToLower() == "code")
            {
                upload = await _context.CodeUploads.SingleOrDefaultAsync(u => u.UploadId == id);
            }
            else if (type.ToLower() == "project")
            {
                upload = await _context.ArchiveUploads.SingleOrDefaultAsync(u => u.UploadId == id);
            }
            else
            {
                return BadRequest();
            }

            if (!CheckReadAccess(upload))
            {
                return Forbid();
            }

            return Ok(upload.TestingFinished);
        }

        [HttpGet("{type}/{id}/result")]
        [ProducesResponseType(200, Type = typeof(IUpload))]
        [ProducesResponseType(400)]
        [ProducesResponseType(403)]
        public async Task<IActionResult> GetResult([FromRoute] string type, [FromRoute] int id)
        {
            IUpload upload;

            if (type.ToLower() == "code")
            {
                upload = await _context.CodeUploads.SingleOrDefaultAsync(u => u.UploadId == id);
            }
            else if (type.ToLower() == "project")
            {
                upload = await _context.ArchiveUploads.SingleOrDefaultAsync(u => u.UploadId == id);
            }
            else
            {
                return BadRequest();
            }

            if (!CheckReadAccess(upload))
            {
                return Forbid();
            }

            if (upload.TestingFinished)
            {
                var result = new { upload.Status, upload.Message, upload.Points };
                return Ok();
            }

            return BadRequest();
        }

        private ApplicationUser GetCurrentUser()
        {
            return null;
        }

        private bool CheckReadAccess(IUpload upload)
        {
            return true;
        }
    }
}