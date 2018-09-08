using System.Collections.Generic;
using Lightest.Data.Models;
using Lightest.Data.Models.TaskModels;

namespace Lightest.Api.ResponseModels
{
    public class CompleteTask
    {
        public Category Category { get; set; }

        public BasicNameViewModel Checker { get; set; }

        public string Description { get; set; }

        public string Examples { get; set; }

        public int Id { get; set; }

        public IEnumerable<BasicLanguage> Languages { get; set; }

        public string Name { get; set; }

        public int Points { get; set; }

        public bool Public { get; set; }

        public ICollection<Test> Tests { get; set; }
    }
}
