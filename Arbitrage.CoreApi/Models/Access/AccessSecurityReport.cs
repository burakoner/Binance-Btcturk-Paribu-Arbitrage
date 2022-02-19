using Arbitrage.CoreApi.Database.Poco;
using Gizza.Extensions;
using System;
using System.Collections.Generic;

namespace Arbitrage.CoreApi.Models.Access
{
    public enum ApiAuthendicationMethod { EpochSignature, AccessToken, ApiKey }
    public enum ApiSecurityType { Public, Private }
    public enum ApiPermission
    {
        /* Public */
        Public,

        /* Private */
        ReadInfo,
        SpotTrading,
        MarginTrading,
        FundTrading,
        Staking,
        Futures,
        Dust,
        Withdrawal
    }

    public class AccessSecurityReport
    {
        public USR_TOKEN ApiTokenPoco { get; set; }
        public USR_DATA UserDataPoco { get; set; }
        public Dictionary<string, string> RequestDictionary { get; set; }
        public ApiAuthendicationMethod AuthendicationMethod { get; set; }
        public ApiSecurityType SecurityType { get; set; }
        public ErrorResponse Error { get; set; }

        public string AccessToken { get; set; }
        public string IpAddress { get; set; }
        public string SecurityKey { get; set; }

        public DateTime ServerTime { get; private set; }
        public long ServerTimeLong => ServerTime.ToUnixTimeMilliSeconds();

        public AccessSecurityReport()
        {
            ApiTokenPoco = null;
            UserDataPoco = null;
            RequestDictionary = new Dictionary<string, string>();
            AuthendicationMethod = ApiAuthendicationMethod.EpochSignature;
            SecurityType = ApiSecurityType.Public;

            AccessToken = string.Empty;
            IpAddress = string.Empty;
            SecurityKey = string.Empty;

            ServerTime = AppStatic.Now;
        }
    }
}
