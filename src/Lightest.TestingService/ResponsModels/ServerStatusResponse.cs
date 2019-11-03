using System.ComponentModel.DataAnnotations;

namespace Lightest.TestingService.ResponsModels
{
    public class ServerStatusResponse
    {
        public string Ip { get; set; }

        [Required]
        public string ServerVersion { get; set; }
    }
}
