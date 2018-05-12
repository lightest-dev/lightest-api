using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Text;

namespace Zetester.Data.Models
{
    public class ApplicationUser: IdentityUser
    {
        public virtual ICollection<UserTask> Tasks { get; set; }

        public virtual ICollection<CategoryUser> AvailableCategories { get; set; }

        public int GroupId { get; set; }

        public virtual Group Group { get; set; }
    }
}
