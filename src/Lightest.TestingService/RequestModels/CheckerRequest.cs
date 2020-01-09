using System.Text.Json;

namespace Lightest.TestingService.RequestModels
{
    public class CheckerRequest : BaseRequest
    {
        public string Id { get; set; }

        public string Code { get; set; }

        public override string Type => "checker";

        public override string ToString() => JsonSerializer.Serialize(this);
    }
}
