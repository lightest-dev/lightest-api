using System.ComponentModel.DataAnnotations;

namespace Lightest.IdentityServer.ViewModels
{
    public class LogOutViewModel
    {
        [Required]
        public string ClientName { get; set; }
    }
}
