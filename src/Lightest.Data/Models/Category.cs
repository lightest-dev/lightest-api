using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Lightest.Data.Models.TaskModels;
using Newtonsoft.Json;

namespace Lightest.Data.Models
{
    public class Category
    {
        [Key]
        public Guid Id { get; set; }

        [Required]
        public string Name { get; set; }

        [JsonIgnore]
        public virtual Category Parent { get; set; }

        public Guid? ParentId { get; set; }

        public bool Public { get; set; }

        [JsonIgnore]
        public virtual ICollection<Category> SubCategories { get; set; }

        [JsonIgnore]
        public virtual ICollection<TaskDefinition> Tasks { get; set; }

        [JsonIgnore]
        public virtual ICollection<CategoryUser> Users { get; set; }
    }
}
