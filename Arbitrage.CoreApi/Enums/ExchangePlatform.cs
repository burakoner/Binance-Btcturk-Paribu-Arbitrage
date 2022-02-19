using Gizza.Data.Attributes;

namespace Arbitrage.CoreApi.Enums
{
    public enum ExchangePlatform
    {
        [EnumLabel("None")]
        None = 0,

        [EnumLabel("Binance")]
        Binance = 1,

        [EnumLabel("Paribu")]
        Paribu = 2,

        [EnumLabel("BtcTurk")]
        BtcTurk = 3,
    }
}