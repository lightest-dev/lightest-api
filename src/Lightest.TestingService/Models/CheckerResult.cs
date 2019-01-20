﻿using System;

namespace Lightest.TestingService.Models
{
    public class CheckerResult
    {
        public Guid UploadId { get; set; }

        public string Type { get; set; }

        public string Message { get; set; }

        public string Status { get; set; }

        public int SuccessfulTests { get; set; }

        public int FailedTest { get; set; }
    }
}