namespace Lightest.TestingService.Requests
{
    public abstract class FileRequest : BaseRequest
    {
        public FileRequest(string filename)
        {
            Filename = filename;
        }

        public FileRequest()
        {
            Filename = string.Empty;
        }

        public string Filename { get; set; }

        public override string Type => "file";

        public abstract string FileType { get; }
    }
}
