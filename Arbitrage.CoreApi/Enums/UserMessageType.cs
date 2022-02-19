using Gizza.Data.Attributes;

namespace Arbitrage.CoreApi.Enums
{
    public enum UserMessageType
    {
        [EnumLabel("None")]
        None = 0,

        [EnumLabel("Telegram")]
        Telegram = 1,
    }
}