namespace Lightest.Api.ResponseModels
{
    public class AccessRightsView
    {
        public bool CanChangeAccess { get; set; }

        public bool CanRead { get; set; }

        public bool CanWrite { get; set; }
    }
}
