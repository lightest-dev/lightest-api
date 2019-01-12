using System.Linq;
using Lightest.AccessService.Interfaces;
using Lightest.Data.Models;
using Lightest.Data.Models.TaskModels;
using Microsoft.AspNetCore.Identity;

namespace Lightest.AccessService.RoleBasedAccessServices
{
    public class TasksAccessService : RoleChecker, IAccessService<TaskDefinition>
    {
        public TasksAccessService(UserManager<ApplicationUser> userManager) : base(userManager)
        {
        }

        public bool CheckAdminAccess(TaskDefinition task, ApplicationUser requester)
        {
            return IsAdmin(requester);
        }

        public bool CheckReadAccess(TaskDefinition task, ApplicationUser requester)
        {
            return task?.Users?.Any(u => u.UserId == requester.Id) == true || IsTeacherOrAdmin(requester)
                           || task?.Public == true;
        }

        public bool CheckWriteAccess(TaskDefinition task, ApplicationUser requester)
        {
            return IsTeacherOrAdmin(requester);
        }
    }
}
