using System;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using Lightest.Data.Mongo.Models;
using Lightest.Data.Mongo.Models.Services;

namespace WebApplication.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UploadDataController : ControllerBase
    {
        private readonly UploadDataService _uploadDataService;

        public UploadDataController(UploadDataService uploadDataService) =>_uploadDataService = uploadDataService;
        
        [HttpGet]
        public ActionResult<List<UploadData>> Get() =>
            _uploadDataService.Get();
        
        [HttpGet("{id:length(24)}", Name = "GetUploadData")]
        public ActionResult<UploadData> Get(string id)
        {
            var book = _uploadDataService.Get(id);

            if (book == null)
            {
                return NotFound();
            }

            return book;
        }
        
        [HttpPost]
        public ActionResult<UploadData> Create(UploadData uploadData)
        {
            _uploadDataService.Add(uploadData);

            return CreatedAtRoute("GetUploadData", new { id = uploadData.Id.ToString() }, uploadData);
        }
        
        [HttpDelete("{id:length(24)}")]
        public IActionResult Delete(string id)
        {
            var book = _uploadDataService.Get(id);

            if (book == null)
            {
                return NotFound();
            }

            _uploadDataService.Delete(book.Id);

            return NoContent();
        }
    }
}
