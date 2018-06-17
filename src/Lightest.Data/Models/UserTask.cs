using Lightest.Data.Models.TaskModels;
using System;

namespace Lightest.Data.Models
{
    public class UserTask
    {
        public string UserId { get; set; }

        public ApplicationUser User { get; set; }

        public int TaskId { get; set; }

        public Task Task { get; set; }

        public AccessRights UserRights { get; set; }

        public DateTime Deadline { get; set; }
    }
}