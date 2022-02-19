using Gizza.Data.Attributes;

namespace Arbitrage.CoreApi.Enums
{
    public enum ArbitrageMode
    {
        [EnumLabel("None")]
        None = 0,

        [EnumLabel("Classic")]
        Classic = 1,

        [EnumLabel("Cross")]
        Cross = 2,
    }
}