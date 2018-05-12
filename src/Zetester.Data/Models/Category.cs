using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Zetester.Data.Models
{
    public class Category
    {
        [Key]
        public int Id { get; set; }

        public string Name { get; set; }

        public int? ParentId { get; set; }

        public virtual Category Parent { get; set; }

        public virtual ICollection<Category> SubCategories { get; set; }

        public virtual ICollection<CategoryUser> Users { get; set; }

        public virtual ICollection<Task> Tasks { get; set; }
    }
}
