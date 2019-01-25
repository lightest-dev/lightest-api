using System.ComponentModel.DataAnnotations;
using Lightest.TestingService.Models;

namespace Lightest.Api.RequestModels
{
    public class TestingError : NewServer
    {
        [Required]
        public string ErrorMessage { get; set; }
    }
}
