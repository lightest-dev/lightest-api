using System;

namespace Lightest.Api.ResponseModels.TaskViews
{
    public class AssignmentView : BasicNameView
    {
        public bool Completed { get; set; }

        public double HighScore { get; set; }

        public DateTime? Deadline { get; set; }
    }
}
