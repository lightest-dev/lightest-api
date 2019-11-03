using System;

namespace Lightest.Api.ResponseModels.TaskViews
{
    public class UserTaskView : BasicNameView
    {
        public bool Completed { get; set; }

        public double HighScore { get; set; }

        public DateTime Deadline { get; set; }
    }
}
