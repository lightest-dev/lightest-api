namespace Lightest.Api.ResponseModels
{
    public class AccessRightsUser : AccessRightsView
    {
        public string Id { get; set; }

        public bool IsOwner { get; set; }

        public string UserName { get; set; }
    }
}
