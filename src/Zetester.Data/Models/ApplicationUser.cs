using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;

namespace Zetester.Data.Models
{
    public class ApplicationUser : IdentityUser
    {
        public virtual ICollection<UserTask> Tasks { get; set; }

        public virtual ICollection<CategoryUser> AvailableCategories { get; set; }

        public int? GroupId { get; set; }

        public virtual Group Group { get; set; }
    }
}