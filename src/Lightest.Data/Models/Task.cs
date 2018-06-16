using Newtonsoft.Json;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Lightest.Data.Models
{
    public class Task
    {
        [Key]
        public int Id { get; set; }

        public virtual Category Category { get; set; }

        public int CategoryId { get; set; }

        public string Name { get; set; }

        public bool Public { get; set; }

        public string Description { get; set; }

        public string Examples { get; set; }

        public int Points { get; set; }

        public int CheckerId { get; set; }

        public virtual Checker Checker { get; set; }

        [JsonIgnore]
        public virtual ICollection<TaskLanguage> Languages { get; set; }

        [JsonIgnore]
        public virtual ICollection<UserTask> Users { get; set; }

        [JsonIgnore]
        public virtual ICollection<Test> Tests { get; set; }
    }
}