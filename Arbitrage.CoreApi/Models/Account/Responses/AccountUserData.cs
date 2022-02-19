using Arbitrage.CoreApi.Database.Poco;
using Newtonsoft.Json;

namespace Arbitrage.CoreApi.Models.Account.Responses
{
    public class AccountUserData
    {
        [JsonProperty("name")]
        public string Name { get; set; }
        [JsonProperty("surname")]
        public string Surname { get; set; }
        [JsonProperty("email")]
        public string Email { get; set; }

        public AccountUserData(USR_DATA poco)
        {
            Name = poco.NAME;
            Surname = poco.SURNAME;
            Email = poco.EMAIL;
        }
    }
}