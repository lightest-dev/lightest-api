using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Zetester.Data.Models
{
    public class Group
    {
        [Key]
        public int Id { get; set; }

        public string Name { get; set; }

        public int? ParentId { get; set; }

        public virtual Group Parent { get; set; }

        public virtual ICollection<Group> SubGroups { get; set; }

        public virtual ICollection<UserGroup> Users { get; set; }
    }
}