using System;

namespace Lightest.Api.ResponseModels.TaskViews
{
    public class AssignmentView : BasicNameView
    {
        public bool Completed { get; set; }

        public double HighScore { get; set; }

        public DateTime? Deadline { get; set; }

        public Guid CategoryId { get; set; }

        public bool Public { get; set; }

        public int Points { get; set; }
    }
}
