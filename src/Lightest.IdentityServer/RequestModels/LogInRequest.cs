namespace Lightest.IdentityServer.RequestModels
{
    public class LogInRequest
    {
        public string Login { get; set; }

        public string Password { get; set; }

        public bool RememberMe { get; set; }
    }
}
