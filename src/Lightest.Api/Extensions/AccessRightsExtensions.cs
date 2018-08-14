using Lightest.Data.Models.TaskModels;

namespace Lightest.Api.Extensions
{
    public static class AccessRightsExtensions
    {
        public static void CopyTo(this IAccessRights original, IAccessRights target)
        {
            target.CanRead = original.CanRead;
            target.CanWrite = original.CanWrite;
            target.CanChangeAccess = original.CanChangeAccess;
        }

        public static void SetFullRights(this IAccessRights rights)
        {
            rights.CanRead = true;
            rights.CanWrite = true;
            rights.CanChangeAccess = true;
            rights.IsOwner = true;
        }
    }
}