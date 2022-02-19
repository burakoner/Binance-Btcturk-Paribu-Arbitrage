using Arbitrage.CoreApi.Attributes;
using Arbitrage.CoreApi.BaseStructure;
using Arbitrage.CoreApi.Enums;

namespace Arbitrage.CoreApi
{
    public class AppSettings : BaseSettings
    {
        public AppSettings(AppConnections appConn) : base(appConn) { }

        #region AppSettingsSection.Register
        [AppSettingsFlag(AppSettingsSection.Telegram, 1001)]
        public string Telegram_Token { get; set; }
        #endregion

    }

}