using Newtonsoft.Json;

namespace Arbitrage.CoreApi.Models.Auth.Requests
{
    public class AuthLoginRequest
    {
        [JsonProperty("email")]
        public string Email { get; set; }

        [JsonProperty("password")]
        public string Password { get; set; }
    }
}
