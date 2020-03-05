using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Lightest.AccessService.Interfaces;
using Lightest.Api.RequestModels.CheckerRequests;
using Lightest.Api.ResponseModels.Checker;
using Lightest.Data;
using Lightest.Data.Models;
using Lightest.Data.Mongo.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Lightest.Api.Controllers
{
    [Produces("application/json")]
    [Route("[controller]")]
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
        [ProducesResponseType(200, Type = typeof(IEnumerable<BasicCheckerView>))]
        public async Task<IActionResult> GetCheckers()
        {
            if (!await _accessService.CanRead(default, await GetCurrentUser()))
            {
                return Forbid();
            }
            return Ok(_context.Checkers
                .AsNoTracking()
                .Select(c => new BasicCheckerView { Id = c.Id, Name = c.Name, Compiled = c.Compiled }));
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

            if (!await _accessService.CanRead(checker.Id, await GetCurrentUser()))
            {
                return Forbid();
            }

            return Ok(checker);
        }

        // POST: api/Checkers
        [HttpPost]
        [ProducesResponseType(403)]
        [ProducesResponseType(201, Type = typeof(Checker))]
        public async Task<IActionResult> PostChecker([FromBody] AddCheckerRequest checker)
        {
            var entry = new Checker
            {
                Name = checker.Name,
                Code = checker.Code
            };

            if (!await _accessService.CanAdd(entry, await GetCurrentUser()))
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
        public async Task<IActionResult> PutChecker([FromRoute] Guid id, [FromBody] UpdateCheckerRequest checker)
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

            if (!await _accessService.CanEdit(entry.Id, await GetCurrentUser()))
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

            if (checker == null)
            {
                return NotFound();
            }

            if (!await _accessService.CanEdit(checker.Id, await GetCurrentUser()))
            {
                return Forbid();
            }

            _context.Checkers.Remove(checker);
            // TODO: catch exception
            await _context.SaveChangesAsync();

            return Ok(checker);
        }
    }
}
