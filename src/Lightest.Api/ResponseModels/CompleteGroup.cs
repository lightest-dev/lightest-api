using System.Collections.Generic;
using Lightest.Data.Models;

namespace Lightest.Api.ResponseModels
{
    public class CompleteGroup
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public Group Parent { get; set; }

        public ICollection<Group> SubGroups { get; set; }

        public IEnumerable<AccessRightsUser> Users { get; set; }
    }
}
