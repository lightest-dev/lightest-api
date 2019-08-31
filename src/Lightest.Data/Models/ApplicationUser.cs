using System.Collections.Generic;
using Lightest.Data.Models.TaskModels;
using Microsoft.AspNetCore.Identity;

namespace Lightest.Data.Models
{
    public class ApplicationUser : IdentityUser
    {
        public string Name { get; set; }

        public string Surname { get; set; }

        public virtual ICollection<CategoryUser> AvailableCategories { get; set; }

        public virtual ICollection<Upload> CodeUploads { get; set; }

        public virtual ICollection<UserGroup> Groups { get; set; }

        public virtual ICollection<UserTask> Tasks { get; set; }
    }
}
