﻿using System.Linq;
using System.Threading.Tasks;
using Lightest.Api.Services;
using Lightest.Api.Services.AccessServices;
using Lightest.Data;
using Lightest.Data.Models;
using Lightest.Data.Models.TaskModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Lightest.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class UploadsController : Controller
    {
        private readonly IAccessService<IUpload> _accessService;
        private readonly RelationalDbContext _context;
        private readonly ITestingService _testingService;
        private readonly UserManager<ApplicationUser> _userManager;

        public UploadsController(ITestingService testingService,
            RelationalDbContext context,
            IAccessService<IUpload> accessService,
            UserManager<ApplicationUser> userManager)
        {
            _testingService = testingService;
            _context = context;
            _accessService = accessService;
            _userManager = userManager;
        }

        [HttpGet("{type}/{id}/status")]
        [ProducesResponseType(200, Type = typeof(bool))]
        [ProducesResponseType(400)]
        [ProducesResponseType(403)]
        public async Task<IActionResult> CheckStatus([FromRoute] string type, [FromRoute] int id)
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

            return Ok(upload.TestingFinished);
        }

        [HttpGet("{type}/{id}/result")]
        [ProducesResponseType(200, Type = typeof(IUpload))]
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

            if (!upload.TestingFinished)
            {
                return BadRequest();
            }

            var result = new { upload.Status, upload.Message, upload.Points };
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
                return BadRequest();
            }

            upload.Task = task;

            if (!_accessService.CheckWriteAccess(upload, user))
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

            var successful = await _testingService.BeginTesting(upload);
            if (successful)
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
            var user = await GetCurrentUser();

            var task = await _context.Tasks
                .Include(t => t.Languages)
                .SingleOrDefaultAsync(t => t.Id == upload.TaskId);

            if (task == null)
            {
                return BadRequest("task");
            }

            upload.Task = task;

            if (!_accessService.CheckWriteAccess(upload, user))
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

            var successful = await _testingService.BeginTesting(upload);
            if (successful)
            {
                return Ok(upload.UploadId);
            }
            return BadRequest();
        }

        private async Task<ApplicationUser> GetCurrentUser()
        {
            var id = User.Claims.SingleOrDefault(c => c.Type == "sub");
            var user = await _userManager.FindByIdAsync(id.Value);
            return user;
        }
    }
}