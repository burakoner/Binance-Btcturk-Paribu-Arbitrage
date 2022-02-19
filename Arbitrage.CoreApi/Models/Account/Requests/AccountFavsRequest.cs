using Arbitrage.CoreApi.Database.Poco;
using Arbitrage.CoreApi.Enums;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace Arbitrage.CoreApi.Models.Account.Requests
{
    public class AccountFavsRequest
    {
        [JsonProperty("mode")]
        public ArbitrageMode Mode { get; set; }
        [JsonProperty("sender")]
        public ExchangePlatform Sender { get; set; }
        [JsonProperty("recipient")]
        public ExchangePlatform Recipient { get; set; }
        [JsonProperty("data")]
        public Dictionary<string, bool> Data { get; set; }
    }
}