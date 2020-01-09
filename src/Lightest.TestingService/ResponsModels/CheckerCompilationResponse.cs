using System;
using System.ComponentModel.DataAnnotations;

namespace Lightest.TestingService.ResponsModels
{
    public class CheckerCompilationResponse
    {
        [Required]
        public Guid Id { get; set; }

        public string Message { get; set; }

        [Required]
        public bool Compiled { get; set; }
    }
}
