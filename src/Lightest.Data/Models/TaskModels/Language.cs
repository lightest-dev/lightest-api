using Newtonsoft.Json;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Lightest.Data.Models.TaskModels
{
    public class Language
    {
        [Key]
        public int Id { get; set; }

        public string Name { get; set; }

        [JsonIgnore]
        public string Extension { get; set; }

        [JsonIgnore]
        public virtual ICollection<TaskLanguage> Tasks { get; set; }

        [JsonIgnore]
        public virtual ICollection<ArchiveUpload> ArchiveUploads { get; set; }

        [JsonIgnore]
        public virtual ICollection<CodeUpload> CodeUploads { get; set; }
    }
}