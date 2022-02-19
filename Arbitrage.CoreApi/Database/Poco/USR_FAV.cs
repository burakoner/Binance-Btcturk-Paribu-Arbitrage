using Arbitrage.CoreApi.Enums;
using Gizza.Data.Database;
using Newtonsoft.Json;

namespace Arbitrage.CoreApi.Database.Poco
{
    [TableName(Tables.USR_FAVS), PrimaryKey("ID")]
    public class USR_FAV : PocoTable<USR_FAV>
    {
        // Constructors
        public USR_FAV() { }
        public USR_FAV(int id, PocoDatabase db) : base(id, db) { }

        // Columns
        [JsonProperty("ID")]
        public int ID { get; set; }
        [JsonProperty("USER_ID")]
        public int USER_ID { get; set; }
        [JsonProperty("MARKET_ID")]
        public int MARKET_ID { get; set; }
        [JsonProperty("EXC_SENDER")]
        public ExchangePlatform EXC_SENDER { get; set; }
        [JsonProperty("EXC_RECIPIENT")]
        public ExchangePlatform EXC_RECIPIENT { get; set; }
        [JsonProperty("MODE")]
        public ArbitrageMode MODE { get; set; }
    }
}