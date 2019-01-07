namespace Lightest.TestingService.Requests
{
    internal class TestRequest : FileRequest
    {
        public TestRequest(string filename) : base(filename)
        {
        }

        public override string FileType => "test";
    }
}
