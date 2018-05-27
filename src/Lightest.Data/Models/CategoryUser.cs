using Newtonsoft.Json;

namespace Lightest.Data.Models
{
    public class CategoryUser
    {
        public string UserId { get; set; }

        [JsonIgnore]
        public ApplicationUser User { get; set; }

        public int CategoryId { get; set; }

        [JsonIgnore]
        public Category Category { get; set; }

        public AccessRights UserRights { get; set; }
    }
}