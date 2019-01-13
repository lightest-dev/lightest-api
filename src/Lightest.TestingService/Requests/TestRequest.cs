namespace Lightest.TestingService.Requests
{
    public class TestRequest : FileRequest
    {
        public TestRequest(string filename) : base(filename)
        {
        }

        public override string FileType => "test";
    }
}
