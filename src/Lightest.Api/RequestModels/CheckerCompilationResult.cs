using System;
using System.ComponentModel.DataAnnotations;

namespace Lightest.Api.RequestModels
{
    public class CheckerCompilationResult
    {
        [Required]
        public Guid Id { get; set; }

        [Required]
        public string Message { get; set; }

        [Required]
        public bool Compiled { get; set; }
    }
}
