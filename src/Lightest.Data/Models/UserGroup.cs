using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;

namespace Lightest.Data.Models
{
    public class UserGroup
    {
        [Required]
        public string UserId { get; set; }

        [JsonIgnore]
        public ApplicationUser User { get; set; }

        [Required]
        public int GroupId { get; set; }

        [JsonIgnore]
        public Group Group { get; set; }
    }
}