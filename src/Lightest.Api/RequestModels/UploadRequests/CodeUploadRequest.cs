using System;
using System.ComponentModel.DataAnnotations;

namespace Lightest.Api.RequestModels.UploadRequests
{
    public class CodeUploadRequest
    {
        [Required]
        public Guid Id { get; set; }

        [Required]
        public string Code { get; set; }

        [Required]
        public Guid LanguageId { get; set; }

        [Required]
        public Guid TaskId { get; set; }
    }
}
