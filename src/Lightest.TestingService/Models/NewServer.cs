using System.ComponentModel.DataAnnotations;
using System.Net;

namespace Lightest.TestingService.Models
{
    public class NewServer
    {
        public IPAddress ServerIp { get; set; }
        
        public string Ip { get; set; }

        [Required]
        public string ServerVersion { get; set; }
    }
}
