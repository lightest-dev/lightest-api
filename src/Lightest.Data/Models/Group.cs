﻿using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;

namespace Lightest.Data.Models
{
    public class Group
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }

        [JsonIgnore]
        public virtual Group Parent { get; set; }

        public int? ParentId { get; set; }

        [JsonIgnore]
        public virtual ICollection<Group> SubGroups { get; set; }

        [JsonIgnore]
        public virtual ICollection<UserGroup> Users { get; set; }
    }
}
