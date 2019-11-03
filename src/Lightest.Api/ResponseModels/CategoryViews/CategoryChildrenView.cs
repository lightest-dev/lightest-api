using System.Collections.Generic;
using Lightest.Data.Models;

namespace Lightest.Api.ResponseModels.CategoryViews
{
    public class CategoryChildrenView
    {
        public IEnumerable<BasicNameView> Tasks { get; set; }

        public IEnumerable<Category> SubCategories { get; set; }
    }
}
