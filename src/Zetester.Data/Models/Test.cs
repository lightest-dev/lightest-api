using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Zetester.Data.Models
{
    public class Test
    {
        [Key]
        public int Id { get; set; }

        public int TaskId { get; set; }

        public virtual Task Task { get; set; }

        public string Input { get; set; }

        public string Output { get; set; }
    }
}
