using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Zetester.Data.Models
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

        public virtual ICollection <TaskLanguage> Languages { get; set; }

        public virtual ICollection<UserTask> Users { get; set; }

        public virtual ICollection<Test> Tests { get; set; }
    }
}
