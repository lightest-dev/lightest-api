using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;

namespace Lightest.Data.Models.TaskModels
{
    public class Test
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int TaskId { get; set; }

        [JsonIgnore]
        public virtual TaskDefinition Task { get; set; }

        [Required]
        public string Input { get; set; }

        [Required]
        public string Output { get; set; }

        [JsonIgnore]
        public bool Cached { get; set; }
    }
}