using Lightest.Data.Models.TaskModels;
using Newtonsoft.Json;
using System;
using System.ComponentModel.DataAnnotations;

namespace Lightest.Data.Models
{
    public class UserTask : IAccessRights
    {
        [Required]
        public string UserId { get; set; }

        [JsonIgnore]
        public ApplicationUser User { get; set; }

        [Required]
        public int TaskId { get; set; }

        [JsonIgnore]
        public TaskDefinition Task { get; set; }

        public DateTime Deadline { get; set; }

        [Required]
        public bool CanRead { get; set; }

        [Required]
        public bool CanWrite { get; set; }

        [Required]
        public bool CanChangeAccess { get; set; }

        [Required]
        public bool IsOwner { get; set; }
    }
}