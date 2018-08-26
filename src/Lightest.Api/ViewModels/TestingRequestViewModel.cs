using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Lightest.Api.ViewModels
{
    public class TestingRequestViewModel
    {
        public int UploadId { get; set; }

        public int MemoryLimit { get; set; }

        public int TimeLimit { get; set; }

        public string Extension { get; set; }

        public string Type { get; set; }

        public int TestsCount { get; set; }
    }
}
