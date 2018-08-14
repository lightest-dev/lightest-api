using Newtonsoft.Json;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Lightest.Data.Models.TaskModels
{
    public class TaskDefinition
    {
        [Key]
        public int Id { get; set; }

        [JsonIgnore]
        public virtual Category Category { get; set; }

        [Required]
        public int CategoryId { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        public bool Public { get; set; }

        public string Description { get; set; }

        public string Examples { get; set; }

        [Required]
        public int Points { get; set; }

        [Required]
        public int CheckerId { get; set; }

        [JsonIgnore]
        public virtual Checker Checker { get; set; }

        [JsonIgnore]
        public virtual ICollection<TaskLanguage> Languages { get; set; }

        [JsonIgnore]
        public virtual ICollection<UserTask> Users { get; set; }

        [JsonIgnore]
        public virtual ICollection<Test> Tests { get; set; }

        [JsonIgnore]
        public virtual ICollection<CodeUpload> CodeUploads { get; set; }

        [JsonIgnore]
        public virtual ICollection<ArchiveUpload> ArchiveUploads { get; set; }
    }
}