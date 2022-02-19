using Arbitrage.CoreApi.Converters;
using Arbitrage.CoreApi.Enums;
using Newtonsoft.Json;

namespace Arbitrage.CoreApi.StreamApi.Objects
{
    /// <summary>
    /// Samples:
    /// {"i":<identifier>, "o":"ping", "p":["<timestamp>"]}
    /// {"i":<identifier>, "o":"pong", "p":["<timestamp>"]}
    /// {"i":<identifier>, "o":"login", "p":["<apikey>", "<timestamp>", "<signature>"]}
    /// {"i":<identifier>, "o":"logout"}
    /// {"i":<identifier>, "o":"subscribe", "p":["Channel.One", "Channel.Two", "Channel.Three"]}
    /// {"i":<identifier>, "o":"unsubscribe", "p":["Channel.One", "Channel.Two", "Channel.Three"]}
    /// {"i":<identifier>, "o":"subscriptions"}
    /// </summary>
    public class StreamRequest
    {
        [JsonProperty("i")]
        public long Identifier { get; set; }

        //[JsonProperty("o")]
        //public string OperationCode { get; set; }

        [JsonProperty("o"), JsonConverter(typeof(EnumLabelConverter<StreamOperationCode>))]
        public StreamOperationCode OperationCode { get; set; }

        [JsonProperty("p")]
        public string[] Params { get; set; }

        [JsonIgnore]
        public bool IsValid
        {
            get
            {
                // Check Op Code
                //if (!OperationCode.ToLowerInvariant().IsIn("ping", "pong", "login", "logout", "subscribe", "unsubscribe", "subscriptions"))
                //    return false;
                if (OperationCode == StreamOperationCode.None)
                {
                    return false;
                }

                /*
                // Get Server Time
                var serverTimeLong = AppStatic.Epoch;

                // Check Timestamp
                if (this.Timestamp <= 0)
                    return false;

                // Check Timestamp <> Server Time
                if (this.Timestamp < (serverTimeLong - 60000))
                    return false;

                // Check Timestamp <> Server Time
                if (this.Timestamp > (serverTimeLong + 1000))
                    return false;

                // Check Receive Window
                if (this.ReceiveWindow < 0 || this.ReceiveWindow > 60000)
                    return false;

                // Fix Receive Window
                if (this.ReceiveWindow < 5000)
                    this.ReceiveWindow = 5000;

                // Check Receive Window - Server Time
                if ((serverTimeLong - this.Timestamp) > this.ReceiveWindow)
                    return false;
                */

                // Return
                return true;

            }
        }

        public static bool TryParse(string json, out StreamRequest request)
        {
            request = null;
            try
            {
                request = JsonConvert.DeserializeObject<StreamRequest>(json);
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
