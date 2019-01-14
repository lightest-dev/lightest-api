using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;

namespace Lightest.Data.Models.TaskModels
{
    public class Language
    {
        [Key]
        public Guid Id { get; set; }

        [Required]
        public string Extension { get; set; }

        [Required]
        public string Name { get; set; }

        [JsonIgnore]
        public virtual ICollection<TaskLanguage> Tasks { get; set; }

        [JsonIgnore]
        public virtual ICollection<ArchiveUpload> ArchiveUploads { get; set; }

        [JsonIgnore]
        public virtual ICollection<CodeUpload> CodeUploads { get; set; }
    }
}
