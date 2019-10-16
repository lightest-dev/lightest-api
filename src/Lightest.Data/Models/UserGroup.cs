using System;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using Lightest.Data.Models.TaskModels;

namespace Lightest.Data.Models
{
    public class UserGroup : IAccessRights
    {
        [Required]
        public bool CanRead { get; set; }

        [Required]
        public bool CanWrite { get; set; }

        [Required]
        public bool CanChangeAccess { get; set; }

        [JsonIgnore]
        public bool IsOwner { get; set; }

        [JsonIgnore]
        public Group Group { get; set; }

        [Required]
        public Guid GroupId { get; set; }

        [JsonIgnore]
        public ApplicationUser User { get; set; }

        [Required]
        public string UserId { get; set; }
    }
}
