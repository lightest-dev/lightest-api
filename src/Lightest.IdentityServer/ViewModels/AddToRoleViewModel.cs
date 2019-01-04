using System.ComponentModel.DataAnnotations;

namespace Lightest.IdentityServer.ViewModels
{
    public class AddToRoleViewModel
    {
        [Required]
        public string UserId { get; set; }

        [Required]
        public string Role { get; set; }
    }
}
