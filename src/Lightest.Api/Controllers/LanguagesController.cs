using Lightest.Data;
using Lightest.Data.Models;
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
        public IEnumerable<Language> GetLanguages()
        {
            return _context.Languages;
        }

        // POST: api/Languages
        [HttpPost]
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

            return CreatedAtAction("GetLanguage", new { id = language.Id }, language);
        }

        // DELETE: api/Languages/5
        [HttpDelete("{id}")]
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