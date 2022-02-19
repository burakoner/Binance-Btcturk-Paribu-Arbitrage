using Newtonsoft.Json;

namespace Arbitrage.CoreApi.Models.Common
{
    public class CodeRequest
    {
        [JsonProperty("code")]
        public string Code { get; set; }
    }
}
