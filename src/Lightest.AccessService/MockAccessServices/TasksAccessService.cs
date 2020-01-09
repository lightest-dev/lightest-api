using Lightest.AccessService.Interfaces;
using Lightest.Data.Models;
using Lightest.Data.Models.TaskModels;

namespace Lightest.AccessService.MockAccessServices
{
    public class TasksAccessService : IAccessService<TaskDefinition>
    {
        public bool HasAdminAccess(ApplicationUser requester) => true;

        public bool HasReadAccess(TaskDefinition task, ApplicationUser requester) => true;

        public bool HasWriteAccess(TaskDefinition task, ApplicationUser requester) => true;
    }
}
