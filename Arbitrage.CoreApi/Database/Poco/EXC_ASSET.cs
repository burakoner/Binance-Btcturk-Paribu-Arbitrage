using Gizza.Data.Database;
using Newtonsoft.Json;

namespace Arbitrage.CoreApi.Database.Poco
{
    [TableName(Tables.EXC_ASSETS), PrimaryKey("ID")]
    public class EXC_ASSET : PocoTable<EXC_ASSET>
    {
        // Constructors
        public EXC_ASSET() { }
        public EXC_ASSET(int id, PocoDatabase db) : base(id, db) { }

        // Columns
        [JsonProperty("ID")]
        public int ID { get; set; }
        [JsonProperty("SYMBOL")]
        public string SYMBOL { get; set; }
        [JsonProperty("BINANCE_SYMBOL")]
        public string BINANCE_SYMBOL { get; set; }
        [JsonProperty("BTCTURK_SYMBOL")]
        public string BTCTURK_SYMBOL { get; set; }
        [JsonProperty("PARIBU_SYMBOL")]
        public string PARIBU_SYMBOL { get; set; }
        [JsonProperty("TRANSFER_ACTIVE")]
        public bool TRANSFER_ACTIVE { get; set; }
        [JsonProperty("TRANSFER_DECIMALS")]
        public int TRANSFER_DECIMALS { get; set; }
        [JsonProperty("TRANSFER_MINIMUM")]
        public decimal TRANSFER_MINIMUM { get; set; }
        [JsonProperty("TRANSFER_MAXIMUM")]
        public decimal TRANSFER_MAXIMUM { get; set; }
    }
}