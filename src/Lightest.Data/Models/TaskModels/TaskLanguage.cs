using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;

namespace Lightest.Data.Models.TaskModels
{
    public class TaskLanguage
    {
        [JsonIgnore]
        public Language Language { get; set; }

        [Required]
        public int LanguageId { get; set; }

        [Required]
        public int MemoryLimit { get; set; }

        [JsonIgnore]
        public TaskDefinition Task { get; set; }

        [Required]
        public int TaskId { get; set; }

        [Required]
        public int TimeLimit { get; set; }
    }
}
