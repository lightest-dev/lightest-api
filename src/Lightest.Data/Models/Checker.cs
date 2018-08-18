using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Lightest.Data.Models.TaskModels;
using Newtonsoft.Json;

namespace Lightest.Data.Models
{
    public class Checker
    {
        [Required]
        [Column(TypeName = "text")]
        public string Code { get; set; }

        [JsonIgnore]
        public bool Compiled { get; set; }

        [Key]
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }

        [JsonIgnore]
        public virtual ICollection<TaskDefinition> Tasks { get; set; }
    }
}
