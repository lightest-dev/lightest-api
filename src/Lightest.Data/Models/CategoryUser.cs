using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;

namespace Lightest.Data.Models
{
    public class CategoryUser
    {
        [Required]
        public string UserId { get; set; }

        [JsonIgnore]
        public ApplicationUser User { get; set; }

        [Required]
        public int CategoryId { get; set; }

        [JsonIgnore]
        public Category Category { get; set; }

        [Required]
        public AccessRights UserRights { get; set; }
    }
}