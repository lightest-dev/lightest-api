using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;

namespace Lightest.Data.Models.TaskModels
{
    public class TaskLanguage
    {
        [Required]
        public int TaskId { get; set; }

        [JsonIgnore]
        public TaskDefinition Task { get; set; }

        [Required]
        public int LanguageId { get; set; }

        [JsonIgnore]
        public Language Language { get; set; }

        [Required]
        public int MemoryLimitation { get; set; }

        [Required]
        public int TimeLimitation { get; set; }
    }
}