using System.Collections.Generic;
using Lightest.Data.Models;

namespace Lightest.Api.ResponseModels
{
    public class CompleteCategory : BasicNameViewModel
    {
        public Category Parent { get; set; }

        public ICollection<Category> SubCategories { get; set; }

        public IEnumerable<BasicNameViewModel> Tasks { get; set; }

        public IEnumerable<AccessRightsUser> Users { get; set; }
    }
}
