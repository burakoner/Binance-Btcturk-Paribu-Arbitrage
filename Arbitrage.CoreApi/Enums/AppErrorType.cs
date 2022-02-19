using Gizza.Data.Attributes;

namespace Arbitrage.CoreApi.Enums
{
    public enum AppErrorType
    {
        //[Label("Unknown")]
        //Unknown = 0,

        [EnumLabel("Error")]
        Error = 1,

        [EnumLabel("Failure")]
        Failure = 2,

        [EnumLabel("Exception")]
        Exception = 3,
    }
}