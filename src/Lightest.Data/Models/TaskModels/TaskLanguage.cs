using Newtonsoft.Json;

namespace Lightest.Data.Models.TaskModels
{
    public class TaskLanguage
    {
        public int TaskId { get; set; }
        [JsonIgnore]
        public TaskDefinition Task { get; set; }

        public int LanguageId { get; set; }
        [JsonIgnore]
        public Language Language { get; set; }

        public int MemoryLimitation { get; set; }

        public int TimeLimitation { get; set; }
    }
}