using Lightest.Data.Models.TaskModels;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Lightest.Data.Models
{
    public class Category
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }

        public int? ParentId { get; set; }

        [JsonIgnore]
        public virtual Category Parent { get; set; }

        [JsonIgnore]
        public virtual ICollection<Category> SubCategories { get; set; }

        [JsonIgnore]
        public virtual ICollection<CategoryUser> Users { get; set; }

        [JsonIgnore]
        public virtual ICollection<TaskDefinition> Tasks { get; set; }
    }
}