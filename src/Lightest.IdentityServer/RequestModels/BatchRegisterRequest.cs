namespace Lightest.IdentityServer.RequestModels
{
    public class BatchRegisterRequest
    {
        public string Prefix { get; set; }

        public uint Count { get; set; }
    }
}
