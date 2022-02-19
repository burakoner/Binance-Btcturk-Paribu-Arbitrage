using Gizza.Data.Database;
using Newtonsoft.Json;

namespace Arbitrage.CoreApi.Database.Poco
{
    [TableName(Tables.USR_TOKENS), PrimaryKey("ID")]
    public class USR_TOKEN : PocoTable<USR_TOKEN>
    {
        // Constructors
        public USR_TOKEN() { }
        public USR_TOKEN(int id, PocoDatabase db) : base(id, db) { }

        // Columns
        [JsonProperty("ID")]
        public int ID { get; set; }
        [JsonProperty("USER_ID")]
        public int USER_ID { get; set; }
        [JsonProperty("LOGIN_AT")]
        public long LOGIN_AT { get; set; }
        [JsonProperty("LOGIN_EXPIRES")]
        public long LOGIN_EXPIRES { get; set; }
        [JsonProperty("LOGIN_IPADDRESS")]
        public string LOGIN_IPADDRESS { get; set; }
        [JsonProperty("LOGIN_ACCESSTOKEN")]
        public string LOGIN_ACCESSTOKEN { get; set; }
    }
}
