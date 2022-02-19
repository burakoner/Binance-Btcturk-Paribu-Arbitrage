using Newtonsoft.Json;

namespace Arbitrage.CoreApi.Models.Common
{
    public class AssetRequest
    {
        [JsonProperty("asset")]
        public string Asset { get; set; }
    }
}
