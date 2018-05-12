using System;
using System.Collections.Generic;
using System.Text;

namespace Zetester.Data.Models
{
    public class TaskLanguage
    {
        public int TaskId { get; set; }
        public Task Task { get; set; }

        public int LanguageId { get; set; }
        public Language Language { get; set; }

        public int MemoryLimitation { get; set; }

        public int TimeLimitation { get; set; }
    }
}
