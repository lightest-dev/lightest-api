using System.Collections.Generic;
using Lightest.Data.Models;

namespace Lightest.Api.ResponseModels
{
    public class CompleteCategory
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public Category Parent { get; set; }

        public ICollection<Category> SubCategories { get; set; }

        public IEnumerable<BasicNameViewModel> Tasks { get; set; }

        public IEnumerable<AccessRightsUser> Users { get; set; }
    }
}
