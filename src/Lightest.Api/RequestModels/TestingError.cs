using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Lightest.TestingService.Models;

namespace Lightest.Api.RequestModels
{
    public class TestingError : NewServer
    {
        [Required]
        public string ErrorMessage { get; set; }
    }
}
