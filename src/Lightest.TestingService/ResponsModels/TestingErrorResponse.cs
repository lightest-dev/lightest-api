using System.ComponentModel.DataAnnotations;

namespace Lightest.TestingService.ResponsModels
{
    public class TestingErrorResponse : ServerStatusResponse
    {
        [Required]
        public string ErrorMessage { get; set; }
    }
}
