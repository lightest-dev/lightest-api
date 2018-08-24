using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;

namespace Lightest.Data.Models.TaskModels
{
    public class Test
    {
        [Key]
        public int Id { get; set; }

        [JsonIgnore]
        public bool Cached { get; set; }

        [Required]
        public string Input { get; set; }

        [Required]
        public string Output { get; set; }

        [JsonIgnore]
        public virtual TaskDefinition Task { get; set; }

        [Required]
        public int TaskId { get; set; }
    }
}
