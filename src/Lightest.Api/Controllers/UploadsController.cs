using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Lightest.AccessService.Interfaces;
using Lightest.Api.ResponseModels;
using Lightest.Data;
using Lightest.Data.Models;
using Lightest.Data.Models.TaskModels;
using Lightest.TestingService.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Lightest.Api.Controllers
{
    [Route("[controller]")]
    [Authorize]
    public class UploadsController : BaseUserController
    {
        private readonly IAccessService<IUpload> _accessService;
        private readonly ITestingService _testingService;

        public UploadsController(
            ITestingService testingService,
            RelationalDbContext context,
            IAccessService<IUpload> accessService,
            UserManager<ApplicationUser> userManager) : base(context, userManager)
        {
            _testingService = testingService;
            _accessService = accessService;
        }

        [HttpGet("{taskId}")]
        [ProducesResponseType(200, Type = typeof(IEnumerable<UserUploadResult>))]
        public async Task<IActionResult> GetLastUploads(int taskId)
        {
            var user = await GetCurrentUser();
            var uploads = _context.CodeUploads
                .AsNoTracking()
                .Where(u => u.UserId == user.Id && u.TaskId == taskId)
                .OrderBy(u => u.UploadId)
                .Take(10)
                .Select(u => new UserUploadResult
                {
                    Id = u.UploadId,
                    Message = u.Message,
                    Status = u.Status,
                    Points = u.Points
                });
            return Ok(uploads);
        }

        [HttpGet("{taskId}/all")]
        [ProducesResponseType(200, Type = typeof(IEnumerable<UserUploadResult>))]
        public async Task<IActionResult> GetAllUploads(int taskId)
        {
            var user = await GetCurrentUser();
            if (!_accessService.CheckWriteAccess(null, user))
            {
                return Forbid();
            }
            var uploads = _context.CodeUploads
                .AsNoTracking()
                .Where(u => u.TaskId == taskId)
                .OrderBy(u => u.UploadId)
                .Select(u => new UserUploadResult
                {
                    Id = u.UploadId,
                    Message = u.Message,
                    Status = u.Status,
                    Points = u.Points
                });
            return Ok(uploads);
        }

        [HttpGet("{type}/{id}/result")]
        [ProducesResponseType(200, Type = typeof(UserUploadResult))]
        [ProducesResponseType(400)]
        [ProducesResponseType(403)]
        public async Task<IActionResult> GetResult([FromRoute] string type, [FromRoute] int id)
        {
            var user = await GetCurrentUser();
            IUpload upload;

            if (type.ToLower() == "code")
            {
                upload = await _context.CodeUploads
                    .AsNoTracking()
                    .SingleOrDefaultAsync(u => u.UploadId == id);
            }
            else if (type.ToLower() == "project")
            {
                upload = await _context.ArchiveUploads
                    .AsNoTracking()
                    .SingleOrDefaultAsync(u => u.UploadId == id);
            }
            else
            {
                return BadRequest();
            }

            if (!_accessService.CheckReadAccess(upload, user))
            {
                return Forbid();
            }

            var result = new UserUploadResult
            {
                Id = id,
                Status = upload.Status,
                Message = upload.Message,
                Points = upload.Points
            };
            return Ok(result);
        }

        [HttpPost("code")]
        [ProducesResponseType(200, Type = typeof(int))]
        [ProducesResponseType(400)]
        [ProducesResponseType(403)]
        public async Task<IActionResult> UploadCode([FromBody] CodeUpload upload)
        {
            var user = await GetCurrentUser();

            var task = await _context.Tasks
                            .Include(t => t.Languages)
                            .SingleOrDefaultAsync(t => t.Id == upload.TaskId);

            if (task == null)
            {
                ModelState.AddModelError(nameof(upload.TaskId), "Task not found");
                return BadRequest(ModelState);
            }

            upload.Task = task;

            if (!_accessService.CheckWriteAccess(upload, user))
            {
                return Forbid();
            }

            var language = await _context.Languages.SingleOrDefaultAsync(l => l.Id == upload.LanguageId);
            if (language == null || task.Languages.All(l => l.LanguageId != upload.LanguageId))
            {
                ModelState.AddModelError(nameof(upload.LanguageId), "Language not found");
                return BadRequest(ModelState);
            }

            upload.Status = UploadStatus.New;
            upload.Points = 0;
            upload.UserId = user.Id;

            _context.CodeUploads.Add(upload);
            await _context.SaveChangesAsync();

            var successful = await _testingService.BeginTesting(upload);
            return Ok(upload.UploadId);
        }

        [HttpPost("project")]
        [ProducesResponseType(200, Type = typeof(int))]
        [ProducesResponseType(400)]
        [ProducesResponseType(403)]
        public async Task<IActionResult> UploadProject([FromBody] ArchiveUpload upload)
        {
            var user = await GetCurrentUser();

            var task = await _context.Tasks
                .Include(t => t.Languages)
                .SingleOrDefaultAsync(t => t.Id == upload.TaskId);

            if (task == null)
            {
                ModelState.AddModelError(nameof(upload.TaskId), "Task not found");
                return BadRequest(ModelState);
            }

            upload.Task = task;

            if (!_accessService.CheckWriteAccess(upload, user))
            {
                return Forbid();
            }

            var language = await _context.Languages.SingleOrDefaultAsync(l => l.Id == upload.LanguageId);
            if (language == null || task.Languages.All(l => l.LanguageId != upload.LanguageId))
            {
                ModelState.AddModelError(nameof(upload.LanguageId), "Language not found");
                return BadRequest(ModelState);
            }

            upload.Status = UploadStatus.New;
            upload.Points = 0;
            upload.UserId = user.Id;
            _context.ArchiveUploads.Add(upload);

            await _context.SaveChangesAsync();

            var successful = await _testingService.BeginTesting(upload);
            if (successful)
            {
                return Ok(upload.UploadId);
            }
            return BadRequest();
        }
    }
}
