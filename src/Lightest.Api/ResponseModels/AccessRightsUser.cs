namespace Lightest.Api.ResponseModels
{
    public class AccessRightsUser
    {
        public bool CanChangeAccess { get; set; }

        public bool CanRead { get; set; }

        public bool CanWrite { get; set; }

        public string Id { get; set; }

        public bool IsOwner { get; set; }

        public string UserName { get; set; }
    }
}
