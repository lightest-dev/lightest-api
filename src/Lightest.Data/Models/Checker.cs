using Lightest.Data.Models.TaskModels;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Lightest.Data.Models
{
    public class Checker
    {
        [Key]
        public int Id { get; set; }

        [JsonIgnore]
        public bool Compiled { get; set; }

        [Column(TypeName = "text")]
        public string Code { get; set; }

        public string Name { get; set; }

        [JsonIgnore]
        public virtual ICollection<Task> Tasks { get; set; }
    }
}
