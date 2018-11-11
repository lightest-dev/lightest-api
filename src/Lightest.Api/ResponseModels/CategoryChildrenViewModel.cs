using System.Collections.Generic;
using Lightest.Data.Models;

namespace Lightest.Api.ResponseModels
{
    public class CategoryChildrenViewModel
    {
        public IEnumerable<BasicNameViewModel> Tasks { get; set; }

        public IEnumerable<Category> SubCategories { get; set; }
    }
}
