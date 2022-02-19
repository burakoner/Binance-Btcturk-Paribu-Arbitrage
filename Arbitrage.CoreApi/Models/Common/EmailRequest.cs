using Newtonsoft.Json;

namespace Arbitrage.CoreApi.Models.Common
{
    public class EmailRequest
    {
        [JsonProperty("email")]
        public string Email { get; set; }
    }
}
