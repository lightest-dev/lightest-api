using System.Text.Json;

namespace Lightest.TestingService.Requests
{
    public class TestRequest : FileRequest
    {
        public TestRequest(string filename) : base(filename)
        {
        }

        public override string FileType => "test";

        public override string ToString()
        {
            return JsonSerializer.Serialize(this);
        }
    }
}
