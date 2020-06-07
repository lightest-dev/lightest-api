using System;
using System.ComponentModel.DataAnnotations;

namespace Lightest.Data.Models
{
    public class CodeUpload
    {
        [Required]
        public Guid UploadId { get; set; }

        [Required]
        public string Code { get; set; }
    }
}
