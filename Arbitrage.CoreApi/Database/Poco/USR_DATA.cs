using Gizza.Data.Database;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace Arbitrage.CoreApi.Database.Poco
{
    [TableName(Tables.USR_DATA), PrimaryKey("ID")]
    public class USR_DATA : PocoTable<USR_DATA>
    {
        // Constructors
        public USR_DATA() { }
        public USR_DATA(int id, PocoDatabase db) : base(id, db) { }

        // Columns
        [JsonProperty("ID")]
        public int ID { get; set; }
        [JsonProperty("EMAIL")]
        public string EMAIL { get; set; }
        [JsonProperty("NAME")]
        public string NAME { get; set; }
        [JsonProperty("SURNAME")]
        public string SURNAME { get; set; }
        [JsonProperty("PASSWORD")]
        public string PASSWORD { get; set; }

        /* Binance */
        [JsonProperty("BINANCE_APIKEY")]
        public string BINANCE_APIKEY { get; set; }
        [JsonProperty("BINANCE_SECRET")]
        public string BINANCE_SECRET { get; set; }
        [JsonProperty("BINANCE_FEE_MAKER")]
        public decimal BINANCE_FEE_MAKER { get; set; }
        [JsonProperty("BINANCE_FEE_TAKER")]
        public decimal BINANCE_FEE_TAKER { get; set; }

        /* BtcTurk */
        [JsonProperty("BTCTURK_APIKEY")]
        public string BTCTURK_APIKEY { get; set; }
        [JsonProperty("BTCTURK_SECRET")]
        public string BTCTURK_SECRET { get; set; }
        [JsonProperty("BTCTURK_FEE_MAKER")]
        public decimal BTCTURK_FEE_MAKER { get; set; }
        [JsonProperty("BTCTURK_FEE_TAKER")]
        public decimal BTCTURK_FEE_TAKER { get; set; }

        /* Paribu */
        [JsonProperty("PARIBU_TOKEN")]
        public string PARIBU_TOKEN { get; set; }
        [JsonProperty("PARIBU_FEE_MAKER")]
        public decimal PARIBU_FEE_MAKER { get; set; }
        [JsonProperty("PARIBU_FEE_TAKER")]
        public decimal PARIBU_FEE_TAKER { get; set; }

        /* Telegram */
        [JsonProperty("TELEGRAM_ACTIVE")]
        public bool TELEGRAM_ACTIVE { get; set; }
        [JsonProperty("TELEGRAM_USER_ID")]
        public long TELEGRAM_USER_ID { get; set; }
        [JsonProperty("TELEGRAM_PERCENT")]
        public decimal TELEGRAM_PERCENT { get; set; }
        [JsonProperty("TELEGRAM_INTERVAL")]
        public int TELEGRAM_INTERVAL { get; set; }
        [JsonProperty("TELEGRAM_CHANNELS")]
        public string TELEGRAM_CHANNELS { get; set; }

    }
}