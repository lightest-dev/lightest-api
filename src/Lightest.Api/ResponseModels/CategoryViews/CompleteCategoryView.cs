using System.Collections.Generic;
using Lightest.Data.Models;

namespace Lightest.Api.ResponseModels.CategoryViews
{
    public class CompleteCategoryView : BasicNameView
    {
        public Category Parent { get; set; }

        public ICollection<Category> SubCategories { get; set; }

        public IEnumerable<BasicNameView> Tasks { get; set; }

        public IEnumerable<AccessRightsUser> Users { get; set; }
    }
}
