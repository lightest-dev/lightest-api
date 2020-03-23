﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Lightest.AccessService.Interfaces;
using Lightest.Api.ResponseModels.UploadViews;
using Lightest.Data;
using Lightest.Data.Models;
using Lightest.Data.Models.TaskModels;
using Lightest.Data.Mongo.Services;
using Lightest.TestingService.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Lightest.Api.Controllers
{
    [Route("[controller]")]
    public class UploadsController : BaseUserController
    {
        private readonly IAccessService<Upload> _accessService;
        private readonly ITestingService _testingService;
        private readonly IUploadDataRepository _uploadDataRepository;

        public UploadsController(
            ITestingService testingService,
            RelationalDbContext context,
            IAccessService<Upload> accessService,
            IUploadDataRepository uploadDataRepository,
            UserManager<ApplicationUser> userManager) : base(context, userManager)
        {
            _testingService = testingService;
            _accessService = accessService;
            _uploadDataRepository = uploadDataRepository;
        }

        [HttpGet("{taskId}")]
        [ProducesResponseType(200, Type = typeof(IEnumerable<LastUploadView>))]
        public async Task<IActionResult> GetLastUploads(Guid taskId)
        {
            var user = await GetCurrentUser();
            var uploads = _context.Uploads
                .AsNoTracking()
                .Where(u => u.UserId == user.Id && u.TaskId == taskId)
                .OrderByDescending(u => u.UploadDate)
                .Take(10)
                .Select(u => new LastUploadView
                {
                    Id = u.Id,
                    Message = u.Message,
                    Status = u.Status,
                    Points = u.Points
                });
            foreach (var upload in uploads)
            {
                upload.Code = _uploadDataRepository.Get(upload.Id)?.Code;
            }
            return Ok(uploads);
        }
 
        [HttpGet("{taskId}/all")]
        [ProducesResponseType(200, Type = typeof(IEnumerable<LastUploadView>))]
        public async Task<IActionResult> GetAllUploads(Guid taskId)
        {
            var user = await GetCurrentUser();
            if (!await _accessService.CanEdit(taskId, user))
            {
                return Forbid();
            }

            var uploads = _context.Uploads
                .AsNoTracking()
                .Where(u => u.TaskId == taskId)
                .OrderByDescending(u => u.UploadDate)
                .Select(u => new LastUploadView
                {
                    Id = u.Id,
                    Message = u.Message,
                    Status = u.Status,
                    Points = u.Points
                });
            foreach (var upload in uploads)
            {
                upload.Code = _uploadDataRepository.Get(upload.Id)?.Code;
            }

            return Ok(uploads);
        }

        [HttpGet("{id}/result")]
        [ProducesResponseType(200, Type = typeof(UploadResultView))]
        [ProducesResponseType(400)]
        [ProducesResponseType(403)]
        public async Task<IActionResult> GetResult([FromRoute] Guid id)
        {
            var user = await GetCurrentUser();
            var upload = await _context.Uploads
                    .AsNoTracking()
                    .SingleOrDefaultAsync(u => u.Id == id);

            if (upload == null)
            {
                return NotFound();
            }

            if (!(await _accessService.CanRead(upload.Id, user)))
            {
                return Forbid();
            }

            var result = new UploadResultView
            {
                Id = id,
                Status = upload.Status,
                Message = upload.Message,
                Points = upload.Points
            };
            return Ok(result);
        }

        [HttpPost("code")]
        [ProducesResponseType(200, Type = typeof(Guid))]
        [ProducesResponseType(400)]
        [ProducesResponseType(403)]
        public async Task<IActionResult> UploadCode([FromBody] Upload upload)
        {
            var user = await GetCurrentUser();

            var task = await _context.Tasks
                            .Include(t => t.Languages)
                            .Include(t => t.Tests)
                            .Include(t => t.Checker)
                            .SingleOrDefaultAsync(t => t.Id == upload.TaskId);

            if (task == null)
            {
                ModelState.AddModelError(nameof(upload.TaskId), "Task not found");
                return BadRequest(ModelState);
            }

            upload.Task = task;

            if (!await _accessService.CanAdd(upload, user))
            {
                return Forbid();
            }

            var language = await _context.Languages.FindAsync(upload.LanguageId);
            if (language == null || task.Languages.All(l => l.LanguageId != upload.LanguageId))
            {
                ModelState.AddModelError(nameof(upload.LanguageId), "Language not found");
                return BadRequest(ModelState);
            }

            upload.Language = language;
            upload.Status = UploadStatus.New;
            upload.Points = 0;
            upload.UserId = user.Id;
            upload.UploadDate = DateTime.Now;

            _context.Uploads.Add(upload);
            await _context.SaveChangesAsync();

            await _testingService.BeginTesting(upload);
            return Ok(upload.Id);
        }
    }
}
