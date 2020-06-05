using System;
using System.ComponentModel.DataAnnotations;

namespace Lightest.Api.RequestModels.AssignmentRequests
{
    public class AssignmentRequest
    {
        [Required]
        public string UserId { get; set; }

        public bool CanRead { get; set; } = true;

        public bool CanWrite { get; set; }

        public bool CanChangeAccess { get; set; }

        public bool IsOwner { get; set; }

        public DateTime? Deadline { get; set; }
    }
}
