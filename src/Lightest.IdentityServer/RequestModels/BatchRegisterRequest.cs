namespace Lightest.IdentityServer.RequestModels
{
    public class BatchRegisterRequest
    {
        public string Prefix { get; set; }

        public int StartIndex { get; set; }

        public int EndIndex { get; set; }
    }
}
