using Gizza.Data.Database;
using Newtonsoft.Json;

namespace Arbitrage.CoreApi.Database.Poco
{
    [TableName(Tables.EXC_MARKETS), PrimaryKey("ID")]
    public class EXC_MARKET : PocoTable<EXC_MARKET>
    {
        // Constructors
        public EXC_MARKET() { }
        public EXC_MARKET(int id, PocoDatabase db) : base(id, db) { }

        // Columns
        [JsonProperty("ID")]
        public int ID { get; set; }
        [JsonProperty("SYMBOL")]
        public string SYMBOL { get; set; }
        [JsonProperty("BASE_ASSET_ID")]
        public int BASE_ASSET_ID { get; set; }
        [JsonProperty("QUOTE_ASSET_ID")]
        public int QUOTE_ASSET_ID { get; set; }


        [JsonProperty("BINANCE_SYMBOL")]
        public string BINANCE_SYMBOL { get; set; }
        [JsonProperty("BINANCE_USDT_MARKET")]
        public string BINANCE_USDT_MARKET { get; set; }
        [JsonProperty("BTCTURK_SYMBOL")]
        public string BTCTURK_SYMBOL { get; set; }
        [JsonProperty("BTCTURK_USDT_MARKET")]
        public string BTCTURK_USDT_MARKET { get; set; }
        [JsonProperty("PARIBU_SYMBOL")]
        public string PARIBU_SYMBOL { get; set; }
        [JsonProperty("PARIBU_USDT_MARKET")]
        public string PARIBU_USDT_MARKET { get; set; }


        [JsonProperty("BINANCE_TO_PARIBU_CLASSIC")]
        public bool BINANCE_TO_PARIBU_CLASSIC { get; set; }
        [JsonProperty("BINANCE_TO_PARIBU_CROSS")]
        public bool BINANCE_TO_PARIBU_CROSS { get; set; }
        [JsonProperty("BINANCE_TO_BTCTURK_CLASSIC")]
        public bool BINANCE_TO_BTCTURK_CLASSIC { get; set; }
        [JsonProperty("BINANCE_TO_BTCTURK_CROSS")]
        public bool BINANCE_TO_BTCTURK_CROSS { get; set; }


        [JsonProperty("BTCTURK_TO_BINANCE_CLASSIC")]
        public bool BTCTURK_TO_BINANCE_CLASSIC { get; set; }
        [JsonProperty("BTCTURK_TO_BINANCE_CROSS")]
        public bool BTCTURK_TO_BINANCE_CROSS { get; set; }
        [JsonProperty("BTCTURK_TO_PARIBU_CLASSIC")]
        public bool BTCTURK_TO_PARIBU_CLASSIC { get; set; }
        [JsonProperty("BTCTURK_TO_PARIBU_CROSS")]
        public bool BTCTURK_TO_PARIBU_CROSS { get; set; }


        [JsonProperty("PARIBU_TO_BINANCE_CLASSIC")]
        public bool PARIBU_TO_BINANCE_CLASSIC { get; set; }
        [JsonProperty("PARIBU_TO_BINANCE_CROSS")]
        public bool PARIBU_TO_BINANCE_CROSS { get; set; }
        [JsonProperty("PARIBU_TO_BTCTURK_CLASSIC")]
        public bool PARIBU_TO_BTCTURK_CLASSIC { get; set; }
        [JsonProperty("PARIBU_TO_BTCTURK_CROSS")]
        public bool PARIBU_TO_BTCTURK_CROSS { get; set; }

    }
}