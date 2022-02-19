using Arbitrage.CoreApi.Enums;
using Gizza.Data.Database;
using Newtonsoft.Json;

namespace Arbitrage.CoreApi.Database.Poco
{
    [TableName(Tables.APP_ERRORS), PrimaryKey("ID")]
    public class APP_ERROR : PocoTable<APP_ERROR>
    {
        // Constructors
        public APP_ERROR() { }
        public APP_ERROR(int id, PocoDatabase db) : base(id, db) { }

        // Columns
        [JsonProperty("ID")]
        public int ID { get; set; }
        [JsonProperty("CAT")]
        public long CAT { get; set; }
        [JsonProperty("TYPE")]
        public AppErrorType TYPE { get; set; }
        [JsonProperty("STATUS")]
        public AppErrorStatus STATUS { get; set; }
        [JsonProperty("LOGLEVEL")]
        public AppErrorLevel LOGLEVEL { get; set; }
        [JsonProperty("CLASS")]
        public string CLASS { get; set; }
        [JsonProperty("METHOD")]
        public string METHOD { get; set; }
        [JsonProperty("VERSION")]
        public string VERSION { get; set; }
        [JsonProperty("EXCEPTION_DATA")]
        public string EXCEPTION_DATA { get; set; }
        [JsonProperty("EXCEPTION_HELPLINK")]
        public string EXCEPTION_HELPLINK { get; set; }
        [JsonProperty("EXCEPTION_HRESULT")]
        public string EXCEPTION_HRESULT { get; set; }
        [JsonProperty("EXCEPTION_MESSAGE")]
        public string EXCEPTION_MESSAGE { get; set; }
        [JsonProperty("EXCEPTION_SOURCE")]
        public string EXCEPTION_SOURCE { get; set; }
        [JsonProperty("EXCEPTION_STACKTRACE")]
        public string EXCEPTION_STACKTRACE { get; set; }
        [JsonProperty("EXCEPTION_TARGETSITE")]
        public string EXCEPTION_TARGETSITE { get; set; }
        [JsonProperty("EXCEPTION_JSONDATA")]
        public string EXCEPTION_JSONDATA { get; set; }
    }
}
