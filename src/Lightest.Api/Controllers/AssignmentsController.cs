using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Lightest.AccessService.Interfaces;
using Lightest.Api.RequestModels.AssignmentRequests;
using Lightest.Api.ResponseModels;
using Lightest.Api.ResponseModels.TaskViews;
using Lightest.Data;
using Lightest.Data.Models;
using Lightest.Data.Models.TaskModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Lightest.Api.Controllers
{
    [Produces("application/json")]
    [Route("[controller]")]
    public class AssignmentsController : BaseUserController
    {
        private readonly IAccessService<TaskDefinition> _accessService;

        public AssignmentsController(
            RelationalDbContext context,
            UserManager<ApplicationUser> userManager,
            IAccessService<TaskDefinition> accessService) : base(context, userManager)
        {
            _accessService = accessService;
        }

        [HttpGet("my")]
        [ProducesResponseType(200, Type = typeof(IEnumerable<AssignmentView>))]
        public IActionResult GetAssignedTasks()
        {
            var id = User.Claims.SingleOrDefault(c => c.Type == "sub").Value;
            var assignedTasks = _context.UserTasks.AsNoTracking()
                .Include(ut => ut.Task)
                .Where(ut => ut.UserId == id);

            return Ok(assignedTasks.Select(t => new AssignmentView
            {
                Id = t.TaskId,
                Name = t.Task.Name,
                Deadline = t.Deadline,
                Completed = t.Completed,
                HighScore = t.HighScore
            }));
        }

        [HttpGet("{id}")]
        [ProducesResponseType(200, Type = typeof(IEnumerable<AccessRightsUser>))]
        [ProducesResponseType(400)]
        [ProducesResponseType(403)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> GetUsers([FromRoute] Guid id)
        {
            var task = await _context.Tasks
                .AsNoTracking()
                .Include(t => t.Users)
                .ThenInclude(u => u.User)
                .SingleOrDefaultAsync(t => t.Id == id);

            if (task == null)
            {
                return NotFound();
            }

            if (!await _accessService.CanEdit(task.Id, await GetCurrentUser()))
            {
                return Forbid();
            }

            return Ok(task.Users.Select(u => new AccessRightsUser
            {
                Id = u.UserId,
                UserName = u.User.UserName,
                CanRead = u.CanRead,
                CanWrite = u.CanWrite,
                CanChangeAccess = u.CanChangeAccess,
                IsOwner = u.IsOwner
            }));
        }

        [HttpPost("{taskId}")]
        [ProducesResponseType(200)]
        [ProducesResponseType(403)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> AddAssignments([FromRoute] Guid taskId, [FromBody] AddOrUpdateAssignmentsRequest request)
        {
            if (taskId != request.TaskId)
            {
                ModelState.AddModelError(nameof(taskId), "Task IDs do not match");
                return BadRequest(ModelState);
            }

            return await AddAssignments(taskId, request.Assignments, true);
        }

        [HttpPost("{taskId}/from-group")]
        [ProducesResponseType(200)]
        [ProducesResponseType(403)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> AddGroupAssignments([FromRoute] Guid taskId, [FromBody] AssignGroupRequest request)
        {
            if (taskId != request.TaskId)
            {
                ModelState.AddModelError(nameof(taskId), "Task IDs do not match");
                return BadRequest(ModelState);
            }

            var assignments = await _context.Groups.Include(g => g.Users)
                .Where(g => g.Id == request.GroupId)
                .SelectMany(g => g.Users
                    .Select(u => new AssignmentRequest
                    {
                        UserId = u.UserId,
                        Deadline = request.Deadline
                    }))
                .ToArrayAsync();

            if (assignments.Length == 0)
            {
                ModelState.AddModelError(nameof(request.GroupId), "No users in group or group does not exist.");
                return NotFound(ModelState);
            }

            return await AddAssignments(taskId, assignments, false);
        }

        private async Task<IActionResult> AddAssignments(Guid taskId, AssignmentRequest[] assignments,
            bool updateExistingAssignments)
        {
            var task = await _context.Tasks
               .Include(t => t.Users)
               .SingleOrDefaultAsync(t => t.Id == taskId);

            if (task == null)
            {
                return NotFound();
            }

            if (!await _accessService.CanEdit(task.Id, await GetCurrentUser()))
            {
                return Forbid();
            }

            foreach (var assignment in assignments)
            {
                var existingAssignment = task.Users.SingleOrDefault(u => u.UserId == assignment.UserId);
                if (existingAssignment == null)
                {
                    existingAssignment = new Assignment
                    {
                        IsOwner = false,
                        TaskId = taskId,
                        UserId = assignment.UserId
                    };
                    task.Users.Add(existingAssignment);
                }
                else if (!updateExistingAssignments)
                {
                    continue;
                }
                UpdateAssignment(existingAssignment, assignment);
            }
            await _context.SaveChangesAsync();
            return Ok();
        }

        private void UpdateAssignment(Assignment existingAssignment, AssignmentRequest newAssignment)
        {
            if (existingAssignment.IsOwner)
            {
                return;
            }

            existingAssignment.Deadline = newAssignment.Deadline;
            existingAssignment.CanRead = newAssignment.CanRead;
            existingAssignment.CanWrite = newAssignment.CanWrite;
            existingAssignment.CanChangeAccess = newAssignment.CanChangeAccess;
        }
    }
}
