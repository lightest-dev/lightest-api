using System.Collections.Generic;
using Lightest.Data.Models.TaskModels;
using Microsoft.AspNetCore.Identity;

namespace Lightest.Data.Models
{
    public class ApplicationUser : IdentityUser
    {
        public virtual ICollection<ArchiveUpload> ArchiveUploads { get; set; }

        public virtual ICollection<CategoryUser> AvailableCategories { get; set; }

        public virtual ICollection<CodeUpload> CodeUploads { get; set; }

        public virtual ICollection<UserGroup> Groups { get; set; }

        public virtual ICollection<UserTask> Tasks { get; set; }
    }
}
