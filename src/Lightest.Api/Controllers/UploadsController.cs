using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Lightest.Api.Services;
using Lightest.Data.Models.TaskModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Lightest.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UploadsController : ControllerBase
    {
        private readonly ITestingService testingService;

        public UploadsController(ITestingService _testingService)
        {
            testingService = _testingService;
        }

        [HttpPost("code")]
        public async Task<IActionResult> UploadCode([FromBody] CodeUpload upload)
        {
            throw new NotImplementedException(); 
        }

        [HttpPost("project")]
        public async Task<IActionResult> UploadProject([FromBody] ArchiveUpload upload)
        {
            throw new NotImplementedException();
        }

        [HttpGet("{type}/{id}/status")]
        public async Task<IActionResult> CheckStatus([FromRoute] string type, [FromRoute] int id)
        {
            if (type.ToLower() == "code")
            {

            }
            else if (type.ToLower() == "project")
            {

            }
            else
            {
                return BadRequest();
            }
            throw new NotImplementedException();
        }

        [HttpGet("{type}/{id}/result")]
        public async Task<IActionResult> GetResult([FromRoute] string type, [FromRoute] int id)
        {
            if (type.ToLower() == "code")
            {

            }
            else if (type.ToLower() == "project")
            {

            }
            else
            {
                return BadRequest();
            }
            throw new NotImplementedException();
        }
    }
}