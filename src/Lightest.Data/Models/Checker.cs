using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using Lightest.Data.Models.TaskModels;

namespace Lightest.Data.Models
{
    public class Checker
    {
        [Key]
        public Guid Id { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        [Column(TypeName = "text")]
        public string Code { get; set; }

        public bool? Compiled { get; set; }

        public string Message { get; set; }

        [JsonIgnore]
        public virtual ICollection<TaskDefinition> Tasks { get; set; }

        [JsonIgnore]
        public virtual ICollection<ServerChecker> Servers { get; set; }
    }
}
