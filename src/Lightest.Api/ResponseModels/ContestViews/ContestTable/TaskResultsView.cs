using System;
using System.Collections.Generic;

namespace Lightest.Api.ResponseModels.ContestViews.ContestTable
{
    public class TaskResultsView
    {
        public Guid TaskId { get; set; }

        public IEnumerable<UserResultView> UserResults { get; set; }
    }
}
