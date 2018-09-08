using Lightest.Data.Models;
using Lightest.Data.Models.TaskModels;

namespace Lightest.Api.Services.AccessServices
{
    public class TasksAccessService : IAccessService<TaskDefinition>
    {
        public bool CheckAdminAccess(TaskDefinition task, ApplicationUser requester)
        {
            return true;
        }

        public bool CheckReadAccess(TaskDefinition task, ApplicationUser requester)
        {
            return true;
        }

        public bool CheckWriteAccess(TaskDefinition task, ApplicationUser requester)
        {
            return true;
        }
    }
}
