using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Lightest.IdentityServer.ViewModels
{
    public class LogInViewModel
    {
        public string Login { get; set; }
        public string Password { get; set; }
        public bool RememberMe { get; set; }
        public string ReturnUrl { get; set; }
    }
}
