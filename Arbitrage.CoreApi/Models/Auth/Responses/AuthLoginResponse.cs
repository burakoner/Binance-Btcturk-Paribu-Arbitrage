using Arbitrage.CoreApi.Database.Poco;
using Newtonsoft.Json;

namespace Arbitrage.CoreApi.Models.Auth.Responses
{
    public class AuthLoginResponse
    {
        [JsonProperty("accountId")]
        public long AccountId { get; set; }
        [JsonProperty("accessToken")]
        public string AccessToken { get; set; }

        public AuthLoginResponse(USR_TOKEN apiToken)
        {
            AccountId = apiToken.USER_ID;
            AccessToken = apiToken.LOGIN_ACCESSTOKEN;
        }
    }
}