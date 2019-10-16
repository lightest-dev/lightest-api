using System;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Lightest.Data.Models.TaskModels
{
    public class TaskLanguage
    {
        [JsonIgnore]
        public Language Language { get; set; }

        [Required]
        public Guid LanguageId { get; set; }

        [JsonIgnore]
        public TaskDefinition Task { get; set; }

        [Required]
        public Guid TaskId { get; set; }

        [Required]
        public int TimeLimit { get; set; }

        [Required]
        public int MemoryLimit { get; set; }
    }
}
