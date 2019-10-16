using System;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Lightest.Data.Models.TaskModels
{
    public class Test
    {
        [Key]
        public Guid Id { get; set; }

        [Required]
        public string Input { get; set; }

        [Required]
        public string Output { get; set; }

        [JsonIgnore]
        public virtual TaskDefinition Task { get; set; }

        [Required]
        public Guid TaskId { get; set; }
    }
}
