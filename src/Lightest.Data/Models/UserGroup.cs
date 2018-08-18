using System.ComponentModel.DataAnnotations;
using Lightest.Data.Models.TaskModels;
using Newtonsoft.Json;

namespace Lightest.Data.Models
{
    public class UserGroup : IAccessRights
    {
        [Required]
        public bool CanChangeAccess { get; set; }

        [Required]
        public bool CanRead { get; set; }

        [Required]
        public bool CanWrite { get; set; }

        [JsonIgnore]
        public Group Group { get; set; }

        [Required]
        public int GroupId { get; set; }

        [JsonIgnore]
        public bool IsOwner { get; set; }

        [JsonIgnore]
        public ApplicationUser User { get; set; }

        [Required]
        public string UserId { get; set; }
    }
}
