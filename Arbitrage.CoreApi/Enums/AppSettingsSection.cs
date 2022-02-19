using Gizza.Data.Attributes;

namespace Arbitrage.CoreApi.Enums
{
    public enum AppSettingsSection
    {
        [EnumLabel("Unknown")]
        Unknown = 0,

        [EnumLabel("Telegram")]
        Telegram = 1000,
    }
}