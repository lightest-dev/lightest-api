using System.Text.Json;

namespace Lightest.TestingService.RequestModels
{
    public class TestRequest : FileRequest
    {
        public TestRequest(string filename) : base(filename)
        {
        }

        public override string FileType => "test";

        public override string ToString() => JsonSerializer.Serialize(this);
    }
}
