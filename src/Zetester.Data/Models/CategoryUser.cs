using System;
using System.Collections.Generic;
using System.Text;

namespace Zetester.Data.Models
{
    public class CategoryUser
    {
        public string UserId { get; set; }

        public ApplicationUser User { get; set; }

        public int CategoryId { get; set; }

        public Category Category { get; set; }

        public AccessRights UserRights { get; set; }
    }
}
