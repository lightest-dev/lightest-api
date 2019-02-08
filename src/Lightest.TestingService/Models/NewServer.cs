using System.ComponentModel.DataAnnotations;

namespace Lightest.TestingService.Models
{
    public class NewServer
    {
        public string Ip { get; set; }

        [Required]
        public string ServerVersion { get; set; }
    }
}
