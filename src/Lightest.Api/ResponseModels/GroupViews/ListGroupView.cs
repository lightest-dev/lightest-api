using System;
using System.Text.Json.Serialization;
using Lightest.Data.Models;

namespace Lightest.Api.ResponseModels.GroupViews
{
    public class ListGroupView : AccessRightsView
    {
        public Guid Id { get; set; }

        public string Name { get; set; }

        public bool Public { get; set; }

        [JsonIgnore]
        public UserGroup User { get; set; }
    }
}
