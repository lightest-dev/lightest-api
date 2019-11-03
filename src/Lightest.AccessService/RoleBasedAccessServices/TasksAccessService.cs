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

        public bool HasAdminAccess(ApplicationUser requester) => IsAdmin(requester);

        public bool HasReadAccess(TaskDefinition task, ApplicationUser requester) => task?.Users?.Any(u => u.UserId == requester.Id) == true || IsTeacherOrAdmin(requester)
                           || task?.Public == true;

        public bool HasWriteAccess(TaskDefinition task, ApplicationUser requester) => IsTeacherOrAdmin(requester);
    }
}
