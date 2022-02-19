using Arbitrage.CoreApi.Enums;
using Gizza.Data.Database;
using Newtonsoft.Json;

namespace Arbitrage.CoreApi.Database.Poco
{
    [TableName(Tables.APP_SETTINGS), PrimaryKey("ID")]
    public class APP_SETTING : PocoTable<APP_SETTING>
    {
        // Constructors
        public APP_SETTING() { }
        public APP_SETTING(int id, PocoDatabase db) : base(id, db) { }

        // Columns
        [JsonProperty("ID")]
        public int ID { get; set; }
        [JsonProperty("CAT")]
        public long CAT { get; set; }
        [JsonProperty("SECTION")]
        public AppSettingsSection SECTION { get; set; }
        [JsonProperty("KEYCODE")]
        public int KEYCODE { get; set; }
        [JsonProperty("VALUE_DECIMAL")]
        public decimal? VALUE_DECIMAL { get; set; }
        [JsonProperty("VALUE_LONG")]
        public long? VALUE_LONG { get; set; }
        [JsonProperty("VALUE_TEXT")]
        public string VALUE_TEXT { get; set; }
    }
}
