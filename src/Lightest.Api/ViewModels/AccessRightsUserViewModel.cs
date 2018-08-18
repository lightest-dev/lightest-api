namespace Lightest.Api.ViewModels
{
    public class AccessRightsUserViewModel
    {
        public bool CanChangeAccess { get; set; }

        public bool CanRead { get; set; }

        public bool CanWrite { get; set; }

        public string Id { get; set; }

        public bool IsOwner { get; set; }

        public string UserName { get; set; }
    }
}
