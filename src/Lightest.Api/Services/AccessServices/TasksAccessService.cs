using Lightest.Data.Models;
using Lightest.Data.Models.TaskModels;

namespace Lightest.Api.Services.AccessServices
{
    public class TasksAccessService : IAccessService<TaskDefinition>
    {
        public bool CheckAdminAccess(TaskDefinition category, ApplicationUser user)
        {
            return true;
        }

        public bool CheckReadAccess(TaskDefinition task, ApplicationUser user)
        {
            return true;
        }

        public bool CheckWriteAccess(TaskDefinition task, ApplicationUser user)
        {
            return true;
        }
    }
}
