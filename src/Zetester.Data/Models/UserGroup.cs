using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Zetester.Data.Models
{
    public class UserGroup
    {
        [Required]
        public string UserId { get; set; }
        public ApplicationUser User { get; set; }

        public int GroupId { get; set; }
        public Group Group { get; set; }
    }
}
