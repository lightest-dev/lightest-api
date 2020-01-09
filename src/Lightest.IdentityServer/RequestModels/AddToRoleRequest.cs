using System.ComponentModel.DataAnnotations;

namespace Lightest.IdentityServer.RequestModels
{
    public class AddToRoleRequest
    {
        [Required]
        public string UserId { get; set; }

        [Required]
        public string Role { get; set; }
    }
}
