using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Text;

namespace Zetester.Data.Models
{
    public class ApplicationUser: IdentityUser
    {
        public virtual ICollection<UserTasks> Tasks { get; set; }
    }
}
