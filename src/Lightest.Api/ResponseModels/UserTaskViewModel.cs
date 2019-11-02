using System;

namespace Lightest.Api.ResponseModels
{
    public class UserTaskViewModel : BasicNameViewModel
    {
        public bool Completed { get; set; }

        public double HighScore { get; set; }

        public DateTime Deadline { get; set; }
    }
}
