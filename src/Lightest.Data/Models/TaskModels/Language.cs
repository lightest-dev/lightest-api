using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

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
        public virtual ICollection<Upload> Uploads { get; set; }
    }
}
