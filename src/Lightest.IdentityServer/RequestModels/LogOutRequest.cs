using System.ComponentModel.DataAnnotations;

namespace Lightest.IdentityServer.RequestModels
{
    public class LogOutRequest
    {
        [Required]
        public string ClientName { get; set; }
    }
}
