using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Lightest.Data.Models
{
    public class Group
    {
        [Key]
        public Guid Id { get; set; }

        [Required]
        public string Name { get; set; }

        public bool Public { get; set; }

        [JsonIgnore]
        public virtual Group Parent { get; set; }

        public Guid? ParentId { get; set; }

        [JsonIgnore]
        public virtual ICollection<Group> SubGroups { get; set; }

        [JsonIgnore]
        public virtual ICollection<UserGroup> Users { get; set; }
    }
}
