using System;
using System.ComponentModel.DataAnnotations;

namespace Lightest.Api.RequestModels.CheckerRequests
{
    public class UpdateCheckerRequest : AddCheckerRequest
    {
        [Required]
        public Guid Id { get; set; }
    }
}
