﻿using System.Collections.Generic;
using Lightest.Data.Models;

namespace Lightest.Api.ViewModels
{
    public class CompleteCategoryViewModel
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public Category Parent { get; set; }

        public ICollection<Category> SubCategories { get; set; }

        public IEnumerable<BasicTaskViewModel> Tasks { get; set; }

        public IEnumerable<AccessRightsUserViewModel> Users { get; set; }
    }
}
