﻿using System.ComponentModel.DataAnnotations;
using Lightest.Data.Models.TaskModels;
using Newtonsoft.Json;

namespace Lightest.Data.Models
{
    public class CategoryUser : IAccessRights
    {
        public bool CanChangeAccess { get; set; }

        public bool CanRead { get; set; }

        public bool CanWrite { get; set; }

        [JsonIgnore]
        public Category Category { get; set; }

        [Required]
        public int CategoryId { get; set; }

        public bool IsOwner { get; set; }

        [JsonIgnore]
        public ApplicationUser User { get; set; }

        [Required]
        public string UserId { get; set; }
    }
}
