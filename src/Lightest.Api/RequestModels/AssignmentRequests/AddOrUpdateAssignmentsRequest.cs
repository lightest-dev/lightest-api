using System;
using System.ComponentModel.DataAnnotations;

namespace Lightest.Api.RequestModels.AssignmentRequests
{
    public class AddOrUpdateAssignmentsRequest
    {
        [Required]
        public Guid TaskId { get; set; }

        [Required]
        public AssignmentRequest[] Assignments { get; set; }
    }
}
