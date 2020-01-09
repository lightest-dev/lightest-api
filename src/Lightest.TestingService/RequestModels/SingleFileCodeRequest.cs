using System.Text.Json;

namespace Lightest.TestingService.RequestModels
{
    public class SingleFileCodeRequest : FileRequest
    {
        public SingleFileCodeRequest(string filename) : base(filename)
        {
        }

        public override string FileType => "code";

        public override string ToString() => JsonSerializer.Serialize(this);
    }
}
