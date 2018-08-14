using Lightest.Data.Models;

namespace Lightest.Api.ViewModels
{
    public class AccessRightsUserViewModel
    {
        public string Id { get; set; }

        public string UserName { get; set; }

        public bool CanRead { get; set; }

        public bool CanWrite { get; set; }

        public bool CanChangeAccess { get; set; }

        public bool IsOwner { get; set; }

        public static AccessRightsUserViewModel FromUser(CategoryUser user)
        {
            var result = new AccessRightsUserViewModel
            {
                Id = user.User.Id,
                UserName = user.User.UserName,
                CanRead = user.CanRead,
                CanWrite = user.CanWrite,
                CanChangeAccess = user.CanChangeAccess,
                IsOwner = user.IsOwner
            };
            return result;
        }
    }
}