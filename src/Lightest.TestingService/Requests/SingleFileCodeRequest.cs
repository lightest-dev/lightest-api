using System.Text.Json;

namespace Lightest.TestingService.Requests
{
    public class SingleFileCodeRequest : FileRequest
    {
        public SingleFileCodeRequest(string filename) : base(filename)
        {
        }

        public override string FileType => "code";

        public override string ToString()
        {
            return JsonSerializer.Serialize(this);
        }
    }
}
