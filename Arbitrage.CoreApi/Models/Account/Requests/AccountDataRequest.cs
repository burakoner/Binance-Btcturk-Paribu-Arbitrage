using Arbitrage.CoreApi.Database.Poco;
using Newtonsoft.Json;

namespace Arbitrage.CoreApi.Models.Account.Requests
{
    public class AccountDataRequest
    {
        [JsonProperty("email")]
        public string Email { get; set; }
        [JsonProperty("name")]
        public string Name { get; set; }
        [JsonProperty("surname")]
        public string Surname { get; set; }
        [JsonProperty("password")]
        public string Password { get; set; }
    }
}