using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Lightest.AccessService.Interfaces;
using Lightest.Api.RequestModels;
using Lightest.Api.ResponseModels;
using Lightest.Data;
using Lightest.Data.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Lightest.Api.Controllers
{
    [Produces("application/json")]
    [Route("[controller]")]
    [Authorize]
    public class CheckersController : BaseUserController
    {
        private readonly IAccessService<Checker> _accessService;

        public CheckersController(
            RelationalDbContext context,
        UserManager<ApplicationUser> userManager,
            IAccessService<Checker> accessService) : base(context, userManager)
        {
            _accessService = accessService;
        }

        // GET: api/Checkers
        [HttpGet]
        [ProducesResponseType(200, Type = typeof(IEnumerable<BaseChecker>))]
        public async Task<IActionResult> GetCheckers()
        {
            if (!_accessService.CheckReadAccess(null, await GetCurrentUser()))
            {
                return Forbid();
            }
            return Ok(_context.Checkers
                .AsNoTracking()
                .Select(c => new BaseChecker { Id = c.Id, Name = c.Name, Compiled = c.Compiled }));
        }

        // GET: api/Checkers/5
        [HttpGet("{id}")]
        [ProducesResponseType(200, Type = typeof(Checker))]
        [ProducesResponseType(403)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> GetChecker([FromRoute] Guid id)
        {
            var checker = await _context.Checkers.FindAsync(id);

            if (checker == null)
            {
                return NotFound();
            }

            if (!_accessService.CheckReadAccess(checker, await GetCurrentUser()))
            {
                return Forbid();
            }

            return Ok(checker);
        }

        // POST: api/Checkers
        [HttpPost]
        [ProducesResponseType(403)]
        [ProducesResponseType(201, Type = typeof(Checker))]
        public async Task<IActionResult> PostChecker([FromBody] CheckerAdd checker)
        {
            var entry = new Checker
            {
                Name = checker.Name,
                Code = checker.Code
            };

            if (!_accessService.CheckWriteAccess(entry, await GetCurrentUser()))
            {
                return Forbid();
            }
            _context.Checkers.Add(entry);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetChecker", new { id = entry.Id }, entry);
        }

        // PUT: api/Checkers/5
        [HttpPut("{id}")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(403)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> PutChecker([FromRoute] Guid id, [FromBody] CheckerUpdate checker)
        {
            if (id != checker.Id)
            {
                return BadRequest();
            }

            var entry = _context.Checkers.Find(checker.Id);

            if (entry == null)
            {
                return NotFound();
            }

            if (!_accessService.CheckWriteAccess(entry, await GetCurrentUser()))
            {
                return Forbid();
            }

            entry.Name = checker.Name;
            entry.Compiled = false;
            entry.Message = null;
            entry.Code = checker.Code;

            var checkersToDelete = _context.CachedCheckers.Where(c => c.CheckerId == checker.Id);
            _context.CachedCheckers.RemoveRange(checkersToDelete);

            await _context.SaveChangesAsync();

            return Ok();
        }

        // DELETE: api/Checkers/5
        [HttpDelete("{id}")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(403)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> DeleteChecker([FromRoute] Guid id)
        {
            var checker = await _context.Checkers.FindAsync(id);

            if (!_accessService.CheckWriteAccess(checker, await GetCurrentUser()))
            {
                return Forbid();
            }

            if (checker == null)
            {
                return NotFound();
            }

            _context.Checkers.Remove(checker);
            //catch exception
            await _context.SaveChangesAsync();

            return Ok(checker);
        }
    }
}
