using System.Collections.Generic;

namespace Lightest.Api.ResponseModels.UserViews
{
    public class CompleteUserView
    {
        public string Name { get; set; }

        public string Surname { get; set; }

        public string Login { get; set; }

        public string Email { get; set; }

        public IEnumerable<BasicNameView> Groups { get; set; }
    }
}
