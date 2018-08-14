using Lightest.Data.Models;
using Lightest.Data.Models.TaskModels;
using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;

namespace Lightest.Api.ViewModels
{
    public class AccessRightsViewModel : IAccessRights
    {
        public string UserId { get; set; }

        [JsonIgnore]
        public ApplicationUser User { get; set; }

        [Required]
        public bool CanRead { get; set; }

        [Required]
        public bool CanWrite { get; set; }

        [Required]
        public bool CanChangeAccess { get; set; }

        [JsonIgnore]
        public bool IsOwner { get; set; }
    }
}