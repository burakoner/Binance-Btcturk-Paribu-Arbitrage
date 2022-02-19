using Newtonsoft.Json;

namespace Arbitrage.CoreApi.Models
{
    public class ErrorResponse
    {
        [JsonProperty("code")]
        public int Code { get; set; }
        [JsonProperty("message")]
        public string Message { get; set; }

        public ErrorResponse()
        {

        }

        public ErrorResponse(int errorCode, string errorMessage)
        {
            Code = errorCode;
            Message = errorMessage;
        }
    }
}
