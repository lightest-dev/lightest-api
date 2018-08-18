using System.ComponentModel.DataAnnotations;
using Lightest.Data.Models;
using Lightest.Data.Models.TaskModels;
using Newtonsoft.Json;

namespace Lightest.Api.ViewModels
{
    public class AccessRightsViewModel : IAccessRights
    {
        [Required]
        public bool CanChangeAccess { get; set; }

        [Required]
        public bool CanRead { get; set; }

        [Required]
        public bool CanWrite { get; set; }

        [JsonIgnore]
        public bool IsOwner { get; set; }

        [JsonIgnore]
        public ApplicationUser User { get; set; }

        public string UserId { get; set; }
    }
}
