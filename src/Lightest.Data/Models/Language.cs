using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Lightest.Data.Models
{
    public class Language
    {
        [Key]
        public int Id { get; set; }

        public string Name { get; set; }

        public virtual ICollection<TaskLanguage> Tasks { get; set; }
    }
}