using Newtonsoft.Json;

namespace Lightest.TestingService.Requests
{
    public abstract class BaseRequest
    {
        public abstract string Type { get; }

        public override string ToString()
        {
            return JsonConvert.SerializeObject(this);
        }
    }
}
