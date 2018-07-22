﻿using System;
using System.Collections.Generic;
using System.Text;

namespace Lightest.Data.Models.TaskModels
{
    public class CheckerResult
    {
        public int TaskId { get; set; }

        public string Status { get; set; }

        public string Message { get; set; }

        public int SuccessfulTests { get; set; }

        public int FailedTest { get; set; }
    }
}
