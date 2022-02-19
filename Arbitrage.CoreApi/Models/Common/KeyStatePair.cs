using Newtonsoft.Json;

namespace Arbitrage.CoreApi.Models.Common
{
    public class KeyStatePair
    {
        /// <summary>
        /// Channel Name, Login etc
        /// </summary>
        [JsonProperty("k")]
        public string Key { get; set; }

        /// <summary>
        /// Subscribed, Logged In etc
        /// </summary>
        [JsonProperty("s")]
        public bool State { get; set; }
    }
}
