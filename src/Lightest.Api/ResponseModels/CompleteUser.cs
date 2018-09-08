using System.Collections.Generic;

namespace Lightest.Api.ResponseModels
{
    public class CompleteUser
    {
        public string Name { get; set; }

        public string Surname { get; set; }

        public string Login { get; set; }

        public string Email { get; set; }

        public IEnumerable<BasicNameViewModel> Tasks { get; set; }

        public IEnumerable<BasicNameViewModel> Groups { get; set; }
    }
}
