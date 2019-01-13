namespace Lightest.TestingService.Requests
{
    public class CheckerRequest : BaseRequest
    {
        public string Id { get; set; }

        public string Code { get; set; }

        public override string Type => "checker";
    }
}
