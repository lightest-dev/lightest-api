﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Lightest.Api.ResponseModels
{
    public class UserTaskViewModel
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public bool Completed { get; set; }

        public double HighScore { get; set; }
    }
}
