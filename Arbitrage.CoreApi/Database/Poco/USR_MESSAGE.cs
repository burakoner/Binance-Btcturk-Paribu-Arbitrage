using Arbitrage.CoreApi.Enums;
using Gizza.Data.Database;
using Newtonsoft.Json;

namespace Arbitrage.CoreApi.Database.Poco
{
    [TableName(Tables.USR_MESSAGES), PrimaryKey("ID")]
    public class USR_MESSAGE : PocoTable<USR_MESSAGE>
    {
        // Constructors
        public USR_MESSAGE() { }
        public USR_MESSAGE(int id, PocoDatabase db) : base(id, db) { }

        // Columns
        [JsonProperty("ID")]
        public int ID { get; set; }
        [JsonProperty("CAT")]
        public long CAT { get; set; }
        [JsonProperty("TYPE")]
        public UserMessageType TYPE { get; set; }
        [JsonProperty("STATUS")]
        public UserMessageStatus STATUS { get; set; }
        [JsonProperty("USER_ID")]
        public int USER_ID { get; set; }
        [JsonProperty("MARKET_ID")]
        public int MARKET_ID { get; set; }
        [JsonProperty("MODE")]
        public ArbitrageMode MODE { get; set; }
        [JsonProperty("SENDER")]
        public ExchangePlatform SENDER { get; set; }
        [JsonProperty("RECIPIENT")]
        public ExchangePlatform RECIPIENT { get; set; }
        [JsonProperty("PROFIT_PERCENT")]
        public decimal PROFIT_PERCENT { get; set; }
        [JsonProperty("MESSAGE")]
        public string MESSAGE { get; set; }
    }
}
