using Lightest.AccessService.Interfaces;
using Lightest.Data.Models;
using Lightest.Data.Models.TaskModels;

namespace Lightest.AccessService.MockAccessServices
{
    public class TasksAccessService : IAccessService<TaskDefinition>
    {
        public bool CheckAdminAccess(TaskDefinition task, ApplicationUser requester) => true;

        public bool CheckReadAccess(TaskDefinition task, ApplicationUser requester) => true;

        public bool CheckWriteAccess(TaskDefinition task, ApplicationUser requester) => true;
    }
}
