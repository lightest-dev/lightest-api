using System;
using System.Collections.Generic;
using System.Text;

namespace Lightest.TestingService.Models
{
    class CheckerRequest
    {
        public string Id { get; set; }

        public string Code { get; set; }

        public string Type => "checker";
    }
}
