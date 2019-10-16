using System;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using Lightest.Data.Models.TaskModels;

namespace Lightest.Data.Models
{
    public class CategoryUser : IAccessRights
    {
        public bool CanRead { get; set; }

        public bool CanWrite { get; set; }

        public bool CanChangeAccess { get; set; }

        public bool IsOwner { get; set; }

        [JsonIgnore]
        public Category Category { get; set; }

        [Required]
        public Guid CategoryId { get; set; }

        [JsonIgnore]
        public ApplicationUser User { get; set; }

        [Required]
        public string UserId { get; set; }
    }
}
