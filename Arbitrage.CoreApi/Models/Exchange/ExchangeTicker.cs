using Newtonsoft.Json;

namespace Arbitrage.CoreApi.Models.Exchange
{
    public class ExchangeTicker
    {
        [JsonProperty("Symbol")]
        public string Symbol { get; set; }
        [JsonProperty("BidPrice")]
        public decimal BidPrice { get; set; }
        [JsonProperty("BidQuantity")]
        public decimal BidQuantity { get; set; }
        [JsonProperty("AskPrice")]
        public decimal AskPrice { get; set; }
        [JsonProperty("AskQuantity")]
        public decimal AskQuantity { get; set; }
    }
}
