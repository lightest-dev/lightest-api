using System;
using System.Collections.Generic;

namespace Lightest.Api.ResponseModels.ContestViews.ContestTable
{
    public class ContestTableView
    {
        public Guid ContestId { get; set; }

        public string Name { get; set; }

        public IEnumerable<TaskResultsView> TaskResults { get; set; }
    }
}
