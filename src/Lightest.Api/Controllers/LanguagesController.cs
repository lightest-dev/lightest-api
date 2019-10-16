using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Lightest.AccessService.Interfaces;
using Lightest.Data;
using Lightest.Data.Models;
using Lightest.Data.Models.TaskModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Lightest.Api.Controllers
{
    [Produces("application/json")]
    [Route("[controller]")]
    public class LanguagesController : BaseUserController
    {
        private readonly IAccessService<Language> _accessService;

        public LanguagesController(
            RelationalDbContext context,
            IAccessService<Language> accessService,
            UserManager<ApplicationUser> userManager) : base(context, userManager) => _accessService = accessService;

        // GET: api/Languages
        [HttpGet]
        [ProducesResponseType(200)]
        public IEnumerable<Language> GetLanguages() => _context.Languages;

        // POST: api/Languages
        [HttpPost]
        [ProducesResponseType(201, Type = typeof(Language))]
        [ProducesResponseType(400)]
        [ProducesResponseType(403)]
        public async Task<IActionResult> PostLanguage([FromBody] Language language)
        {
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
        public async Task<IActionResult> DeleteLanguage([FromRoute] Guid id)
        {
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
    }
}
