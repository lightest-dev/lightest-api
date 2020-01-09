using System;
using System.Text.Json.Serialization;
using Lightest.Data.Models;

namespace Lightest.Api.ResponseModels.CategoryViews
{
    public class ListCategoryView : AccessRightsView
    {
        public Guid Id { get; set; }

        public string Name { get; set; }

        public bool Public { get; set; }

        [JsonIgnore]
        public CategoryUser User { get; set; }
    }
}
