using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Lightest.Api.ResponseModels;
using Lightest.Data;
using Lightest.Data.Models;
using Lightest.TestingService.Interfaces;
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
        private readonly IServerRepository _serverRepository;

        public CheckersController(
            RelationalDbContext context,
            IServerRepository serverRepository,
            UserManager<ApplicationUser> userManager) : base(context, userManager)
        {
            _serverRepository = serverRepository;
        }

        // GET: api/Checkers
        [HttpGet]
        public IEnumerable<BasicNameViewModel> GetCheckers()
        {
            return _context.Checkers
                .AsNoTracking()
                .Select(c => new BasicNameViewModel { Id = c.Id, Name = c.Name });
        }

        // GET: api/Checkers/5
        [HttpGet("{id}")]
        [ProducesResponseType(200, Type = typeof(Checker))]
        [ProducesResponseType(403)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> GetChecker([FromRoute] int id)
        {
            var checker = await _context.Checkers.FindAsync(id);

            if (checker == null)
            {
                return NotFound();
            }

            return Ok(checker);
        }

        // POST: api/Checkers
        [HttpPost]
        [ProducesResponseType(403)]
        [ProducesResponseType(201, Type = typeof(Checker))]
        public async Task<IActionResult> PostChecker([FromBody] Checker checker)
        {
            _context.Checkers.Add(checker);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetChecker", new { id = checker.Id }, checker);
        }

        // PUT: api/Checkers/5
        [HttpPut("{id}")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(403)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> PutChecker([FromRoute] int id, [FromBody] Checker checker)
        {
            if (id != checker.Id)
            {
                return BadRequest();
            }

            if (!CheckerExists(id))
            {
                return NotFound();
            }

            _context.Entry(checker).State = EntityState.Modified;

            await _context.SaveChangesAsync();

            _serverRepository.RemoveCachedCheckers(checker.Id);

            return Ok();
        }

        // DELETE: api/Checkers/5
        [HttpDelete("{id}")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(403)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> DeleteChecker([FromRoute] int id)
        {
            var checker = await _context.Checkers.FindAsync(id);
            if (checker == null)
            {
                return NotFound();
            }

            _context.Checkers.Remove(checker);
            //catch exception
            await _context.SaveChangesAsync();

            _serverRepository.RemoveCachedCheckers(id);

            return Ok(checker);
        }

        private bool CheckerExists(int id)
        {
            return _context.Checkers.Any(e => e.Id == id);
        }
    }
}
