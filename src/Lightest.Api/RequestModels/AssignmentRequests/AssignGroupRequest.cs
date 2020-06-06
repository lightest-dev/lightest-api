using System;
using System.ComponentModel.DataAnnotations;

namespace Lightest.Api.RequestModels.AssignmentRequests
{
    public class AssignGroupRequest
    {
        [Required]
        public Guid TaskId { get; set; }

        [Required]
        public Guid GroupId { get; set; }

        public DateTime? Deadline { get; set; }
    }
}
