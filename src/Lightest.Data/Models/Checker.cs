using Lightest.Data.Models.TaskModels;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Lightest.Data.Models
{
    public class Checker
    {
        [Key]
        public int Id { get; set; }

        [JsonIgnore]
        public bool Compiled { get; set; }

        [Required]
        [Column(TypeName = "text")]
        public string Code { get; set; }

        [Required]
        public string Name { get; set; }

        [JsonIgnore]
        public virtual ICollection<TaskDefinition> Tasks { get; set; }
    }
}