using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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
    [Produces("application/json")]
    [Route("api/[controller]")]
    [Authorize]
    public class LanguagesController : Controller
    {
        private readonly IAccessService<Language> _accessService;
        private readonly RelationalDbContext _context;

        public LanguagesController(RelationalDbContext context, IAccessService<Language> accessService)
        {
            _context = context;
            _accessService = accessService;
        }

        // GET: api/Languages
        [HttpGet]
        [ProducesResponseType(200)]
        public IEnumerable<Language> GetLanguages()
        {
            return _context.Languages;
        }

        // POST: api/Languages
        [HttpPost]
        [ProducesResponseType(201, Type = typeof(Language))]
        [ProducesResponseType(400)]
        [ProducesResponseType(403)]
        public async Task<IActionResult> PostLanguage([FromBody] Language language)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var currentUser = await GetCurrentUser();

            if (!_accessService.CheckWriteAccess(language, currentUser))
            {
                return Forbid();
            }

            _context.Languages.Add(language);
            await _context.SaveChangesAsync();

            return Ok(language.Id);
        }

        // DELETE: api/Languages/5
        [HttpDelete("{id}")]
        [ProducesResponseType(200, Type = typeof(Language))]
        [ProducesResponseType(400)]
        [ProducesResponseType(403)]
        [ProducesResponseType(404)]
        [ProducesResponseType(409)]
        public async Task<IActionResult> DeleteLanguage([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var language = await _context.Languages.FindAsync(id);
            if (language == null)
            {
                return NotFound();
            }

            var currentUser = await GetCurrentUser();

            if (!_accessService.CheckWriteAccess(language, currentUser))
            {
                return Forbid();
            }

            try
            {
                _context.Languages.Remove(language);
                await _context.SaveChangesAsync();
            }
            catch
            {
                return StatusCode(409);
            }

            return Ok(language);
        }

        private async Task<ApplicationUser> GetCurrentUser()
        {
            var id = User.Claims.SingleOrDefault(c => c.Type == "sub");
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == id.Value);
            return user;
        }

        private bool LanguageExists(int id)
        {
            return _context.Languages.Any(e => e.Id == id);
        }
    }
}
