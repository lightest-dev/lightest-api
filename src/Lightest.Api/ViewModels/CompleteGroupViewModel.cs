using System.Collections.Generic;
using Lightest.Data.Models;

namespace Lightest.Api.ViewModels
{
    public class CompleteGroupViewModel
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public Group Parent { get; set; }

        public ICollection<Group> SubGroups { get; set; }

        public IEnumerable<AccessRightsUserViewModel> Users { get; set; }
    }
}
