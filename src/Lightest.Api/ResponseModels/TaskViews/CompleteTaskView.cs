using System.Collections.Generic;
using Lightest.Api.ResponseModels.Checker;
using Lightest.Api.ResponseModels.Language;
using Lightest.Data.Models;
using Lightest.Data.Models.TaskModels;

namespace Lightest.Api.ResponseModels.TaskViews
{
    public class CompleteTaskView : BasicNameView
    {
        public Category Category { get; set; }

        public BasicCheckerView Checker { get; set; }

        public string Description { get; set; }

        public string Examples { get; set; }

        public IEnumerable<BasicLanguageView> Languages { get; set; }

        public int Points { get; set; }

        public bool Public { get; set; }

        public ICollection<Test> Tests { get; set; }
    }
}
