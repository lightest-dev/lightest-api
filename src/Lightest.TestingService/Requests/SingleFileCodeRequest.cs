namespace Lightest.TestingService.Requests
{
    internal class SingleFileCodeRequest : FileRequest
    {
        public SingleFileCodeRequest(string filename) : base(filename)
        {
        }

        public override string FileType => "code";
    }
}
