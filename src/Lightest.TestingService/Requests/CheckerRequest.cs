using System.Text.Json;

namespace Lightest.TestingService.Requests
{
    public class CheckerRequest : BaseRequest
    {
        public string Id { get; set; }

        public string Code { get; set; }

        public override string Type => "checker";

        public override string ToString()
        {
            return JsonSerializer.Serialize(this);
        }
    }
}
