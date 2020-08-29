using System.Collections.Generic;
using Lightest.Data.Models;

namespace Lightest.Api.ResponseModels.GroupViews
{
    public class CompleteGroupView : BasicNameView
    {
        public bool Public { get; set; }

        public Group Parent { get; set; }

        public ICollection<Group> SubGroups { get; set; }

        public IEnumerable<AccessRightsUser> Users { get; set; }
    }
}
