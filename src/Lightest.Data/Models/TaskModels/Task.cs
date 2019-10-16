using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Lightest.Data.Models.TaskModels
{
    public class TaskDefinition
    {
        [Key]
        public Guid Id { get; set; }

        [JsonIgnore]
        public virtual Category Category { get; set; }

        [Required]
        public Guid CategoryId { get; set; }

        [JsonIgnore]
        public virtual Checker Checker { get; set; }

        [Required]
        public Guid CheckerId { get; set; }

        [JsonIgnore]
        public virtual ICollection<Upload> CodeUploads { get; set; }

        public string Description { get; set; }

        public string Examples { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        public int Points { get; set; }

        [Required]
        public bool Public { get; set; }

        [JsonIgnore]
        public virtual ICollection<Test> Tests { get; set; }

        [JsonIgnore]
        public virtual ICollection<UserTask> Users { get; set; }

        [JsonIgnore]
        public virtual ICollection<TaskLanguage> Languages { get; set; }
    }
}
