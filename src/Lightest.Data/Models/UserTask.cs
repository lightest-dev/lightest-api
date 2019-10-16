using System;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using Lightest.Data.Models.TaskModels;

namespace Lightest.Data.Models
{
    public class UserTask : IAccessRights
    {
        [Required]
        public bool CanRead { get; set; }

        [Required]
        public bool CanWrite { get; set; }

        [Required]
        public bool CanChangeAccess { get; set; }

        [Required]
        public bool IsOwner { get; set; }

        public DateTime Deadline { get; set; }

        [JsonIgnore]
        public TaskDefinition Task { get; set; }

        [Required]
        public Guid TaskId { get; set; }

        [JsonIgnore]
        public ApplicationUser User { get; set; }

        [Required]
        public string UserId { get; set; }

        [JsonIgnore]
        public bool Completed { get; set; }

        [JsonIgnore]
        public double HighScore { get; set; }
    }
}
