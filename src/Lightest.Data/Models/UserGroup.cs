using System.ComponentModel.DataAnnotations;

namespace Lightest.Data.Models
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