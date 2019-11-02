using System.Text.Json;

namespace Lightest.TestingService.Requests
{
    public abstract class FileRequest : BaseRequest
    {
        protected FileRequest(string filename) => Filename = filename;

        protected FileRequest() => Filename = "";

        public string Filename { get; set; }

        public override string Type => "file";

        public abstract string FileType { get; }

        public override string ToString() => JsonSerializer.Serialize(this);
    }
}
