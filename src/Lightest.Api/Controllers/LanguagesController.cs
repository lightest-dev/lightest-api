using Lightest.Data;
using Lightest.Data.Models.TaskModels;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Lightest.Api.Controllers
{
    [Produces("application/json")]
    [Route("api/[controller]")]
    public class LanguagesController : Controller
    {
        private readonly RelationalDbContext _context;

        public LanguagesController(RelationalDbContext context)
        {
            _context = context;
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

            if (!CheckWriteAccess())
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

            if (!CheckWriteAccess())
            {
                return Forbid();
            }

            var language = await _context.Languages.FindAsync(id);
            if (language == null)
            {
                return NotFound();
            }

            try
            {
                _context.Languages.Remove(language);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                return StatusCode(409);
            }

            return Ok(language);
        }

        private bool LanguageExists(int id)
        {
            return _context.Languages.Any(e => e.Id == id);
        }

        private bool CheckWriteAccess()
        {
            return true;
        }
    }
}