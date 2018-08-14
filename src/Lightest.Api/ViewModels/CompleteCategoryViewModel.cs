using Lightest.Data.Models;
using System.Collections.Generic;
using System.Linq;

namespace Lightest.Api.ViewModels
{
    public class CompleteCategoryViewModel
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public Category Parent { get; set; }

        public ICollection<Category> SubCategories { get; set; }

        public IEnumerable<AccessRightsUserViewModel> Users { get; set; }

        public IEnumerable<BasicTaskViewModel> Tasks { get; set; }

        public static CompleteCategoryViewModel FromCategory(Category category)
        {
            var result = new CompleteCategoryViewModel
            {
                Id = category.Id,
                Name = category.Name,
                Parent = category.Parent,
                SubCategories = category.SubCategories,
                Users = category.Users.Select(u => AccessRightsUserViewModel.FromUser(u)),
                Tasks = category.Tasks.Select(t => BasicTaskViewModel.FromTask(t))
            };
            return result;
        }
    }
}