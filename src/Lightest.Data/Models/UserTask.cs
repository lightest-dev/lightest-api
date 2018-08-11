using Lightest.Data.Models.TaskModels;
using Newtonsoft.Json;
using System;
using System.ComponentModel.DataAnnotations;

namespace Lightest.Data.Models
{
    public class UserTask
    {
        [Required]
        public string UserId { get; set; }

        [JsonIgnore]
        public ApplicationUser User { get; set; }

        [Required]
        public int TaskId { get; set; }

        [JsonIgnore]
        public TaskDefinition Task { get; set; }

        [Required]
        public AccessRights UserRights { get; set; }

        public DateTime Deadline { get; set; }
    }
}