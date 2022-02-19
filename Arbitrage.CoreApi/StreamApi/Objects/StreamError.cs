using Newtonsoft.Json;

namespace Arbitrage.CoreApi.StreamApi.Objects
{
    public class StreamError
    {
        [JsonProperty("c")]
        public int Code { get; set; }
        [JsonProperty("m")]
        public string Message { get; set; }

        public StreamError()
        {

        }

        public StreamError(int code, string message)
        {
            Code = code;
            Message = message;
        }
    }
}
