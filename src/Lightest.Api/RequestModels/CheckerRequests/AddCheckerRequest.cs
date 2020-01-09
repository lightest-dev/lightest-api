using System.ComponentModel.DataAnnotations;

namespace Lightest.Api.RequestModels.CheckerRequests
{
    public class AddCheckerRequest
    {
        [Required]
        public string Name { get; set; }

        [Required]
        public string Code { get; set; }
    }
}
